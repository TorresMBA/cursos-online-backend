using System.IO;
using System.Threading;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class ExportPDF
    {
        public class Consulta : IRequest<Stream>{ }

        public class Manejador : IRequestHandler<Consulta, Stream>
        {
            private readonly CursosOnlineContext _context;

            public Manejador(CursosOnlineContext context){
                _context = context;
            }

            public async Task<Stream> Handle(Consulta request, CancellationToken cancellationToken)
            {
                var cursos = await _context.Curso.ToListAsync();
                
                //Fuente de la letra para el pdf
                Font fuenteTitulo = new Font(Font.HELVETICA, 8f, Font.BOLD, BaseColor.Blue);
                Font fuenteHeader = new Font(Font.HELVETICA, 7f, Font.BOLD, BaseColor.Black);
                Font fuenteData = new Font(Font.HELVETICA, 7f, Font.NORMAL, BaseColor.Black);

                //Representacion del archivo pdf es el Stream
                MemoryStream workStream = new MemoryStream();
                
                //El tamaño de pdf que quiero => A4 A3 A2 
                Rectangle rect = new Rectangle(PageSize.A4);

                //Vamos a crear el documento pdf en si
                //La instancia de este objeto nos pide el tamaño, margen izquierda derecha y margenes arriba abajo 
                Document document = new Document(rect, 0, 0, 50, 100);

                //La instancia del objeto que nos permite escribir dentro de él
                PdfWriter writer = PdfWriter.GetInstance(document, workStream);
                writer.CloseStream = false;

                //Abrir el documento para agregarle lo que deseo dentro del reporte
                document.Open();
                document.AddTitle("Lista de Cursos en la Universidad");
                PdfPTable tabla = new PdfPTable(1);//se le indica cuantas columnas tendra la tabla
                tabla.WidthPercentage = 90; //Indicarle el porcentaje de ancho que va a copntener el documento
                PdfPCell celda = new PdfPCell(new Phrase("Lista de Cursos de SQL Server", fuenteTitulo));//crear una celda y como parametro la frase que quiero incluir
                celda.Border = Rectangle.NO_BORDER;// de esta forma mi celda no tendra bordes
                tabla.AddCell(celda);//Por defecto viene con border definidos 
                document.Add(tabla);//Para añadir al documento

                PdfPTable tablaCursos = new PdfPTable(2);//se imprimira dos columnas por eso numero 2
                float[] width = new float[]{40,60};//indicar el ancho de cada una de las celdas, nombre = 40, descripcion = 60 tamaño
                tablaCursos.SetWidthPercentage(width, rect);
                PdfPCell celdaHeaderTitulo = new PdfPCell(new Phrase("Curso", fuenteHeader));
                tablaCursos.AddCell(celdaHeaderTitulo);
                PdfPCell celdaHeaderDescripcion = new PdfPCell(new Phrase("Descripcion", fuenteHeader));
                tablaCursos.AddCell(celdaHeaderDescripcion);
                tablaCursos.WidthPercentage=90;

                foreach(var cursoElemento in cursos){
                    PdfPCell cellDataTitulo = new PdfPCell(new Phrase(cursoElemento.Titulo, fuenteData));
                    tablaCursos.AddCell(cellDataTitulo);

                    PdfPCell cellDataDescripcion = new PdfPCell(new Phrase(cursoElemento.Descripcion, fuenteData));
                    tablaCursos.AddCell(cellDataDescripcion);
                }

                document.Add(tablaCursos);
                document.Close();

                //Transforma de tipo pdf osea stream a un array de bytes[]
                byte[] byteData = workStream.ToArray(); 
                workStream.Write(byteData, 0, byteData.Length);

                workStream.Position = 0;

                return workStream;
            }
        }
    }
}