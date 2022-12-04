#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using forms = System.Windows.Forms;

#endregion

namespace Session04Challenge
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {


        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            //Get Started

            TaskDialog.Show("Hello", "Select Model Lines");

            //Declare Variables
            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements");

            TaskDialog.Show("Confirmation", "you selected " + pickList.Count.ToString());

            List<CurveElement> linelist = new List<CurveElement>();

            foreach (Element element in pickList)
            {
                if (element is CurveElement)
                {
                    CurveElement curve = element as CurveElement;

                    if (curve.CurveElementType == CurveElementType.ModelCurve)
                        linelist.Add((CurveElement)element);


                }

          
            }
             Transaction t = new Transaction(doc);
            t.Start("Create Wall");
            Level newLevel = Level.Create(doc, 15);
            foreach (CurveElement Currentcurve in linelist) {

                Curve Curve1 = Currentcurve.GeometryCurve;
                Wall newWall = Wall.Create(doc, Curve1, newLevel.Id, false);
            }

            t.Commit();
            t.Dispose();
            TaskDialog.Show("Confirmation", "you eselected " + linelist.Count + " lines.");
            return Result.Succeeded;
        }
    }
}
