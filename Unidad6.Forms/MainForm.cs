using System.Data;
using System.Globalization;
using System.Text;
namespace Unidad6.Forms;
/// <summary>Búsquedas con traza, conteos y experimento reproducible.</summary>
public partial class MainForm : Form
{
    private bool ocupado;
    public MainForm() { InitializeComponent(); FormClosing+=AlCerrar; }
    private void AlCerrar(object? sender,FormClosingEventArgs e)
    { if(ocupado) { e.Cancel=true; MessageBox.Show(this,"Espera a que termine el experimento."); } }
    private void Activar(bool valor) { ocupado=!valor; panelAcciones.Enabled=valor; UseWaitCursor=!valor; }
    private void Generar_Click(object? sender,EventArgs e)
    {
        var a=Experimento.Generar((int)cantidad.Value,Random.Shared.Next());
        datos.Text=string.Join(", ",a.Select(x=>x.Clave)); clave.Text=a[0].Clave.ToString();
        cubetas.Value=TablaHash.PrimoDesde(a.Length);
    }
    private void Buscar_Click(object? sender,EventArgs e)
    {
        try
        {
            int[] numeros=Interfaz.Leer(datos.Text);
            if(numeros.Distinct().Count()!=numeros.Length) throw new ArgumentException("Usa claves únicas para comparar la misma posición en los tres métodos.");
            if(!int.TryParse(clave.Text,out int objetivo)) throw new ArgumentException("La clave debe ser un entero.");
            var a=numeros.Select((v,i)=>new Registro(v,i)).ToArray();
            var ordenados=Busqueda.OrdenarCopia(a);
            var h=new TablaHash((int)cubetas.Value); foreach(var r in a) h.Insertar(r);
            var texto=new StringBuilder();
            var t=new DataTable(); t.Columns.Add("Método");
            foreach(string c in new[]{"Posición original","Comparaciones","Iteraciones","Evaluaciones hash"}) t.Columns.Add(c,typeof(int));
            void Agregar(string nombre,Func<Action<string>,Resultado> ejecutar)
            {
                texto.AppendLine("--- "+nombre+" ---"); var r=ejecutar(s=>texto.AppendLine(s));
                t.Rows.Add(nombre,r.Posicion,r.Comparaciones,r.Iteraciones,r.EvaluacionesHash);
                texto.AppendLine(r.Encontrado?"Encontrado en posición original "+r.Posicion:"No encontrado (-1)");
            }
            Agregar("Secuencial",tr=>Busqueda.Secuencial(a,objetivo,tr));
            Agregar("Binaria",tr=>Busqueda.Binaria(ordenados,objetivo,tr));
            Agregar("Hash encadenado",tr=>h.Buscar(objetivo,tr));
            texto.AppendLine($"\r\nHash: M={h.Capacidad}; carga={h.Carga:F3}; inserciones con colisión={h.InsercionesConColision}; comparaciones de construcción={h.ComparacionesConstruccion}.");
            texto.AppendLine("h(k)=((k % M)+M)%M; cubeta de la clave: "+h.Indice(objetivo));
            texto.AppendLine("Cubetas (máximo 30 mostradas):");
            foreach(var linea in h.Describir().Take(30)) texto.AppendLine(linea);
            texto.AppendLine("Ordenamiento y construcción se realizan antes de contar la búsqueda. Los índices comienzan en cero.");
            tabla.DataSource=t; salida.Text=texto.ToString(); estado.Text="Búsqueda terminada. Cambia M para observar colisiones.";
        }
        catch(Exception ex) { MessageBox.Show(this,ex.Message,"Revisar datos"); }
    }
    private async void Experimento_Click(object? sender,EventArgs e)
    {
        try
        {
            string? carpeta=Interfaz.Carpeta(this,"busquedas"); if(carpeta is null) return;
            Activar(false); salida.Clear(); estado.Text="Ejecutando consultas reproducibles…";
            IProgress<string> avance=new Progress<string>(s=>salida.AppendText(s+Environment.NewLine));
            var resumen=await Task.Run(()=>
            {
                Experimento.Ejecutar(carpeta,s=>avance.Report(s));
                var filas=File.ReadLines(Path.Combine(carpeta,"consultas.csv")).Skip(1).Select(x=>x.Split(',')).ToArray();
                var t=new DataTable();
                foreach(string c in new[]{"N","Tipo","Método","Consultas","Comparaciones promedio","Iteraciones promedio","Comparaciones máximas"}) t.Columns.Add(c);
                foreach(var g in filas.GroupBy(x=>new { N=x[0],Tipo=x[3],Metodo=x[5] }))
                    t.Rows.Add(g.Key.N,g.Key.Tipo,g.Key.Metodo,g.Count(),g.Average(x=>double.Parse(x[8],CultureInfo.InvariantCulture)).ToString("F3",CultureInfo.InvariantCulture),g.Average(x=>double.Parse(x[9],CultureInfo.InvariantCulture)).ToString("F3",CultureInfo.InvariantCulture),g.Max(x=>int.Parse(x[8])));
                return t;
            });
            tabla.DataSource=resumen; estado.Text="Resultados: "+carpeta;
            salida.AppendText("\r\nDatos crudos y estadísticas guardados en: "+carpeta+"\r\nLa tabla agrupa las semillas por N, tipo de consulta y método. La prueba adversaria de colisiones se conserva en su archivo separado.");
        }
        catch(Exception ex) { estado.Text="Experimento interrumpido; puede haber archivos parciales."; MessageBox.Show(this,ex.Message,"Experimento"); }
        finally { Activar(true); }
    }
    private void Guardar_Click(object? sender,EventArgs e)
    { try { Interfaz.Exportar(this,tabla); } catch(Exception ex) { MessageBox.Show(this,ex.Message); } }
}
