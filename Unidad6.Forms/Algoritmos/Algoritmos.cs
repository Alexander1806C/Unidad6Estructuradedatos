namespace Unidad6;

/// <summary>La posición siempre corresponde al arreglo original, incluso tras ordenar.</summary>
public readonly record struct Registro(int Clave, int Posicion);
/// <summary>Comparaciones cuenta == y &lt; entre claves; no condiciones de índices/null.</summary>
public readonly record struct Resultado(int Posicion, int Comparaciones, int Iteraciones, int EvaluacionesHash = 0)
{
    public bool Encontrado => Posicion >= 0;
}

public static class Busqueda
{
    public static Resultado Secuencial(Registro[] datos, int clave, Action<string>? traza = null)
    {
        int pasos = 0;
        foreach (var r in datos)
        {
            pasos++;
            traza?.Invoke($"i={pasos - 1}, clave={r.Clave}, comparación == {clave}");
            if (r.Clave == clave) return new(r.Posicion, pasos, pasos);
        }
        return new(-1, pasos, pasos);
    }

    /// <summary>Precondición: arreglo ascendente por Clave y acceso O(1). No ordena ni valida cada consulta.</summary>
    public static Resultado Binaria(Registro[] ordenados, int clave, Action<string>? traza = null)
    {
        int bajo = 0, alto = ordenados.Length - 1, comparaciones = 0, pasos = 0;
        while (bajo <= alto)
        {
            int medio = bajo + (alto - bajo) / 2;
            pasos++; comparaciones++;
            traza?.Invoke($"bajo={bajo}, alto={alto}, medio={medio}, clave={ordenados[medio].Clave}");
            if (ordenados[medio].Clave == clave) return new(ordenados[medio].Posicion, comparaciones, pasos);
            comparaciones++;
            if (ordenados[medio].Clave < clave) bajo = medio + 1;
            else alto = medio - 1;
        }
        return new(-1, comparaciones, pasos);
    }

    public static Registro[] OrdenarCopia(Registro[] datos)
    {
        var copia = (Registro[])datos.Clone();
        Array.Sort(copia, (a, b) => a.Clave.CompareTo(b.Clave));
        return copia;
    }
}

/// <summary>Hash por división con encadenamiento propio; claves únicas, capacidad fija de cubetas.</summary>
public sealed class TablaHash
{
    private sealed class Nodo(Registro dato, Nodo? siguiente)
    {
        public Registro Dato = dato;
        public Nodo? Siguiente = siguiente;
    }
    private readonly Nodo?[] cubetas;
    public int Capacidad => cubetas.Length;
    public int Cantidad { get; private set; }
    public double Carga => (double)Cantidad / Capacidad;
    /// <summary>Inserciones nuevas cuya cubeta ya estaba ocupada (no pares colisionantes).</summary>
    public int InsercionesConColision { get; private set; }
    public long ComparacionesConstruccion { get; private set; }
    public TablaHash(int capacidad)
    {
        if (capacidad < 1) throw new ArgumentOutOfRangeException(nameof(capacidad));
        cubetas = new Nodo?[capacidad];
    }
    /// <summary>Normaliza el módulo sin Math.Abs(int.MinValue) ni desbordamiento en la suma.</summary>
    public int Indice(int clave) => (int)(((long)clave % Capacidad + Capacidad) % Capacidad);
    public bool Insertar(Registro dato)
    {
        int i = Indice(dato.Clave);
        for (var p = cubetas[i]; p != null; p = p.Siguiente)
        {
            ComparacionesConstruccion++;
            if (p.Dato.Clave == dato.Clave) return false; // Conserva la primera posición.
        }
        if (cubetas[i] != null) InsercionesConColision++;
        cubetas[i] = new Nodo(dato, cubetas[i]); Cantidad++;
        return true;
    }
    public Resultado Buscar(int clave, Action<string>? traza = null)
    {
        int i = Indice(clave), pasos = 0;
        traza?.Invoke($"h({clave})={i}; capacidad={Capacidad}");
        for (var p = cubetas[i]; p != null; p = p.Siguiente)
        {
            pasos++; traza?.Invoke($"nodo={pasos}, clave={p.Dato.Clave}");
            if (p.Dato.Clave == clave) return new(p.Dato.Posicion, pasos, pasos, 1);
        }
        return new(-1, pasos, pasos, 1);
    }
    public bool Eliminar(int clave)
    {
        int i = Indice(clave); Nodo? anterior = null;
        for (var p = cubetas[i]; p != null; anterior = p, p = p.Siguiente)
        {
            if (p.Dato.Clave != clave) continue;
            if (anterior == null) cubetas[i] = p.Siguiente;
            else anterior.Siguiente = p.Siguiente;
            Cantidad--; return true;
        }
        return false;
    }
    public int[] Longitudes()
    {
        var r = new int[Capacidad];
        for (int i = 0; i < r.Length; i++)
            for (var p = cubetas[i]; p != null; p = p.Siguiente) r[i]++;
        return r;
    }
    public string[] Describir()
    {
        var r = new string[Capacidad];
        for (int i = 0; i < r.Length; i++)
        {
            var claves = new List<int>();
            for (var p = cubetas[i]; p != null; p = p.Siguiente) claves.Add(p.Dato.Clave);
            r[i] = $"Cubeta {i}: " + (claves.Count == 0 ? "vacía" : string.Join(", ", claves));
        }
        return r;
    }
    public static int PrimoDesde(int minimo)
    {
        for (int n = Math.Max(2, minimo); ; n++)
        {
            bool primo = true;
            for (int d = 2; (long)d * d <= n; d++) if (n % d == 0) { primo = false; break; }
            if (primo) return n;
        }
    }
}
