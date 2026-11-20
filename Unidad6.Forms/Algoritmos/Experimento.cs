using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using System.Runtime.InteropServices;
namespace Unidad6;
public static class Experimento
{
    public static Registro[] Generar(int n, int semilla)
    {
        // Fisher-Yates parcial: muestra sin reemplazo de pares en [-4n,4n-2].
        var rnd = new Random(semilla);
        int[] universo = Enumerable.Range(0, 4*n).Select(i => 2*i-4*n).ToArray();
        var datos = new Registro[n];
        for (int i=0;i<n;i++) { int j=rnd.Next(i,universo.Length); (universo[i],universo[j])=(universo[j],universo[i]); datos[i]=new(universo[i],i); }
        return datos;
    }
    public static void Ejecutar(string carpeta, Action<string>? progreso = null)
    {
        if (Directory.Exists(carpeta) && Directory.EnumerateFileSystemEntries(carpeta).Any())
            throw new IOException("Usa una carpeta nueva o vacía para conservar resultados anteriores.");
        Directory.CreateDirectory(carpeta);
        var opciones=new JsonSerializerOptions { WriteIndented=true };
        File.WriteAllText(Path.Combine(carpeta,"entorno.json"),JsonSerializer.Serialize(new {
            fechaUtc=DateTime.UtcNow, runtime=RuntimeInformation.FrameworkDescription, so=RuntimeInformation.OSDescription,
            arquitectura=RuntimeInformation.ProcessArchitecture.ToString(), semillas=new[]{601,602,603}, tamanos=new[]{1000,10000,100000},
            consultasPorConjunto=1000, presentes=500, ausentes=500, medida="Conteos exactos; no se cronometró tiempo de ejecución"
        },opciones));
        using var crudo = new StreamWriter(Path.Combine(carpeta,"consultas.csv"));
        using var estructura = new StreamWriter(Path.Combine(carpeta,"tablas_hash.csv"));
        using var huellas = new StreamWriter(Path.Combine(carpeta,"SHA256.txt"));
        crudo.WriteLine("N,Semilla,Consulta,Tipo,Clave,Metodo,M,Posicion,Comparaciones,Iteraciones,EvaluacionesHash");
        estructura.WriteLine("N,Semilla,CargaObjetivo,M,CargaReal,CubetasOcupadas,LongitudMaxima,InsercionesConColision,ComparacionesConstruccion");
        int total=0;
        foreach(int n in new[]{1000,10000,100000}) foreach(int semilla in new[]{601,602,603})
        {
            var a=Generar(n,semilla); var b=Busqueda.OrdenarCopia(a);
            string nombre=$"datos_n{n}_s{semilla}.csv", ruta=Path.Combine(carpeta,nombre);
            using(var w=new StreamWriter(ruta)) {w.WriteLine("Clave,Posicion");foreach(var r in a)w.WriteLine($"{r.Clave},{r.Posicion}");}
            huellas.WriteLine(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(ruta)))+"  "+nombre);
            double[] cargas=[0.5,1,2]; var tablas=new TablaHash[3];
            for(int j=0;j<3;j++)
            {
                var h=tablas[j]=new TablaHash(TablaHash.PrimoDesde((int)Math.Ceiling(n/cargas[j])));
                foreach(var dato in a) if(!h.Insertar(dato))throw new InvalidOperationException("Dato duplicado en generador");
                var longitudes=h.Longitudes();
                estructura.WriteLine(FormattableString.Invariant($"{n},{semilla},{cargas[j]},{h.Capacidad},{h.Carga:R},{longitudes.Count(x=>x>0)},{longitudes.Max()},{h.InsercionesConColision},{h.ComparacionesConstruccion}"));
            }
            // Consultas idénticas para todas las técnicas; alterna aciertos y fallos.
            // Los fallos impares caen dentro del rango generador y no pueden existir entre los pares.
            var rnd=new Random(semilla+10000+n);
            for(int q=0;q<1000;q++)
            {
                bool presente=q%2==0; int posicion=presente?rnd.Next(n):-1;
                int clave=presente?a[posicion].Clave:2*rnd.Next(4*n)-4*n+1;
                void Guardar(string metodo,int m,Resultado r)
                {
                    if(r.Posicion!=posicion)throw new InvalidOperationException($"Resultado incorrecto: {n}, {q}, {metodo}");
                    crudo.WriteLine($"{n},{semilla},{q},{(presente?"Presente":"Ausente")},{clave},{metodo},{m},{r.Posicion},{r.Comparaciones},{r.Iteraciones},{r.EvaluacionesHash}");total++;
                }
                Guardar("Secuencial",0,Busqueda.Secuencial(a,clave));
                Guardar("Binaria",0,Busqueda.Binaria(b,clave));
                for(int j=0;j<3;j++)Guardar("Hash_"+cargas[j].ToString(CultureInfo.InvariantCulture),tablas[j].Capacidad,tablas[j].Buscar(clave));
            }
            progreso?.Invoke($"N={n}; semilla={semilla}: 5000 resultados correctos.");
        }
        using(var w=new StreamWriter(Path.Combine(carpeta,"colisiones_adversas.csv")))
        {
            w.WriteLine("N,M,Tipo,Clave,Comparaciones,Iteraciones");
            const int n=200,m=211;var h=new TablaHash(m);
            for(int i=0;i<n;i++)h.Insertar(new(i*m,i));
            for(int i=0;i<=n;i++)
            {
                var r=h.Buscar(i*m);if(r.Posicion!=(i==n?-1:i))throw new InvalidOperationException("Fallo caso adverso");
                w.WriteLine($"{n},{m},{(i==n?"Ausente":"Presente")},{i*m},{r.Comparaciones},{r.Iteraciones}");
            }
            progreso?.Invoke($"Adverso: {n} claves en una cubeta; ausente={h.Buscar(n*m).Comparaciones} comparaciones; colisiones={h.InsercionesConColision}.");
        }
        progreso?.Invoke($"TOTAL: {total} búsquedas principales verificadas y 201 búsquedas adversas. Resultados: {Path.GetFullPath(carpeta)}");
    }
}
