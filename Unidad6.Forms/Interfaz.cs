using System.Data;
using System.Globalization;
using System.Text;
namespace Unidad6.Forms;
internal static class Interfaz
{
    public static int[] Leer(string texto)
    {
        var partes=texto.Split(new[]{',',';',' ','\r','\n','\t'},StringSplitOptions.RemoveEmptyEntries);
        if(partes.Length==0 || partes.Length>1000) throw new ArgumentException("Escribe entre 1 y 1000 enteros separados por comas.");
        return partes.Select(x=>int.TryParse(x,out var n)?n:throw new ArgumentException("Valor inválido: "+x)).ToArray();
    }
    public static string? Carpeta(IWin32Window owner,string unidad)
    {
        using var d=new FolderBrowserDialog { Description="Selecciona dónde guardar los resultados. Se creará una carpeta nueva.", UseDescriptionForTitle=true };
        if(d.ShowDialog(owner)!=DialogResult.OK) return null;
        string ruta=Path.Combine(d.SelectedPath,unidad+"_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+"_"+Guid.NewGuid().ToString("N")[..6]);
        Directory.CreateDirectory(ruta); return ruta;
    }
    public static void Exportar(IWin32Window owner, DataGridView tabla)
    {
        if(tabla.DataSource is not DataTable datos || datos.Rows.Count==0) throw new InvalidOperationException("Primero genera resultados.");
        using var d=new SaveFileDialog { Filter="Archivo CSV|*.csv", FileName="resultados.csv" };
        if(d.ShowDialog(owner)!=DialogResult.OK) return;
        static string Celda(object? v)=>"\""+(Convert.ToString(v,CultureInfo.InvariantCulture)??"").Replace("\"","\"\"")+"\"";
        using var w=new StreamWriter(d.FileName,false,new UTF8Encoding(true));
        w.WriteLine(string.Join(",",datos.Columns.Cast<DataColumn>().Select(c=>Celda(c.ColumnName))));
        foreach(DataRow fila in datos.Rows) w.WriteLine(string.Join(",",fila.ItemArray.Select(Celda)));
    }
}
