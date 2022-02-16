using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITraining_lab3._4
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Pipe> pipes = new FilteredElementCollector(doc)
             .OfClass(typeof(Pipe))
             .Cast<Pipe>()
             .ToList();

            foreach (var pipe in pipes)
            {
                var pipeOutsideDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble();
                var pipeInsideDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM).AsDouble();
                pipeOutsideDiameter = UnitUtils.ConvertFromInternalUnits(pipeOutsideDiameter, UnitTypeId.Millimeters);
                pipeInsideDiameter = UnitUtils.ConvertFromInternalUnits(pipeInsideDiameter, UnitTypeId.Millimeters);

                using (Transaction ts1 = new Transaction(doc, "Set parameters"))
                {
                    ts1.Start();

                    Parameter commentParameter = pipe.LookupParameter("Наименование");
                    commentParameter.Set($"Труба {Math.Round(pipeOutsideDiameter, 1)} мм / {Math.Round(pipeInsideDiameter, 1)} мм");
                    ts1.Commit();
                }
            }
            return Result.Succeeded;
        }
    }
}
