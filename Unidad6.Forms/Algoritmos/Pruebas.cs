namespace Unidad6;
public static class Pruebas
{
    public static void Ejecutar()
    {
        int verificaciones = 0;
        void Exigir(bool ok, string caso) { if (!ok) throw new InvalidOperationException("FALLO: " + caso); verificaciones++; }
        var rnd = new Random(606);
        for (int caso = 0; caso < 75; caso++)
        {
            // Oráculo independiente solo en pruebas, nunca dentro de los algoritmos evaluados.
            int[] valores = caso == 0 ? [] : caso == 1 ? [int.MinValue, 0, int.MaxValue, -1] : Enumerable.Range(0, rnd.Next(1, 100)).Select(_ => rnd.Next(-100,101)).Distinct().ToArray();
            var a = valores.Select((v, i) => new Registro(v, i)).ToArray();
            var b = Busqueda.OrdenarCopia(a); var h = new TablaHash(caso % 9 + 1);
            foreach (var dato in a) Exigir(h.Insertar(dato), "insertar único");
            foreach (int clave in valores.Concat(new[] { -200, 200, int.MinValue, int.MaxValue }))
            {
                int esperado = Array.IndexOf(valores, clave);
                var s = Busqueda.Secuencial(a, clave); var bin = Busqueda.Binaria(b, clave); var hash = h.Buscar(clave);
                Exigir(s.Posicion == esperado && bin.Posicion == esperado && hash.Posicion == esperado, "posición exacta");
                Exigir(s.Comparaciones == s.Iteraciones && hash.Comparaciones == hash.Iteraciones && hash.EvaluacionesHash == 1, "contadores sec/hash");
                Exigir(bin.Comparaciones == 2 * bin.Iteraciones - (bin.Encontrado ? 1 : 0), "contadores binaria");
                int limite = a.Length == 0 ? 0 : (int)Math.Floor(Math.Log2(a.Length)) + 1;
                Exigir(bin.Iteraciones <= limite, "cota binaria");
            }
            Exigir(h.Longitudes().Sum() == a.Length, "integridad de nodos");
        }
        var t = new TablaHash(5);
        Exigir(t.Insertar(new(1,0)) && t.Insertar(new(6,1)) && t.Insertar(new(11,2)), "cadena");
        Exigir(t.Buscar(1) == new Resultado(0,3,3,1), "cola cadena");
        Exigir(t.Buscar(16) == new Resultado(-1,3,3,1), "ausente misma cubeta");
        Exigir(t.Buscar(2) == new Resultado(-1,0,0,1), "cubeta vacía");
        Exigir(!t.Insertar(new(6,99)) && t.Cantidad == 3 && t.Buscar(6).Posicion == 1, "duplicado conserva posición");
        Exigir(t.Eliminar(6) && !t.Buscar(6).Encontrado && t.Buscar(1).Encontrado, "eliminar medio");
        Exigir(t.Eliminar(11) && t.Eliminar(1) && t.Cantidad == 0 && !t.Eliminar(99), "eliminar cabeza cola ausente");
        Exigir(t.Indice(int.MinValue) >= 0 && t.Indice(int.MaxValue) < 5, "módulo extremos");
        bool rechazo = false; try { _ = new TablaHash(0); } catch (ArgumentOutOfRangeException) { rechazo = true; }
        Exigir(rechazo, "capacidad inválida");
        var uno = new Registro[] { new(7,0) };
        Exigir(Busqueda.Binaria(uno,7)==new Resultado(0,1,1), "contador acierto único");
        Exigir(Busqueda.Binaria(uno,8)==new Resultado(-1,2,1), "contador fallo único");
        Console.WriteLine($"RESULTADO: {verificaciones} verificaciones correctas; 0 fallos. Semilla 606; 75 conjuntos.");
        Console.WriteLine("Incluye vacío, un elemento, negativos, extremos int32, colisiones, duplicados, eliminación y contadores exactos.");
    }
}
