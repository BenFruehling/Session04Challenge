#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using forms = System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static Autodesk.Revit.DB.SpecTypeId;

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

            //1.prompt user to select documents

            TaskDialog.Show("Hello", "Select Model Lines");
            
            // 2.filter selected elements
            // 3.Get Level and types
            //4.Loop Through Selected Curve Element
            // 5.Get Graphic Style for curve and curve element
            //6.Use Swithch Statement to create walls, ducts and pipes
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
 
            WallType glazingWT = GetWallTypeByName(doc, "Storefront");
            WallType solidWT = GetWallTypeByName2(doc, "Generic - 8\"");

            MEPSystemType pipeSystemType = GetMEPSystemTypeByName(doc, "Domestic Hot Water");
            PipeType pipeType = GetPipeTypeByName(doc, "Default");
            MEPSystemType ductSystemType = GetMEPSystemTypeByName(doc, "Supply Air");
            DuctType ductType = GetDuctTypeByName(doc, "Default");

            foreach (CurveElement Currentcurve in linelist)
            {

                Curve Curve1 = Currentcurve.GeometryCurve;
               //XYZ startPoint = Currentcurve.GeometryCurve.GetEndPoint( 0 );
               //XYZ endPoint = Currentcurve.GeometryCurve.GetEndPoint( 1 );
                GraphicsStyle currentGS = Currentcurve.LineStyle as GraphicsStyle;

               
                switch(currentGS.Name)
                {
                    case "A-GLAZ":
                //A-GLAZ
                Wall newWall = Wall.Create(doc, Curve1, glazingWT.Id, newLevel.Id, 20, 0, false, false); 
                        break;

                    case "A-WALL":
                //Generic 8" Wall
                Wall newWall2 = Wall.Create(doc, Curve1, solidWT.Id, newLevel.Id, 20, 0, false, false);

                        break;
                    case "M-DUCT":
                        //M-Duct
                Duct newDuct = Duct.Create(doc, ductSystemType.Id, ductType.Id, newLevel.Id, Curve1.GetEndPoint(0), Curve1.GetEndPoint(1));                        
                        break;
                    case "P-PIPE":
                //P-PIPE
                Pipe newPipe = Pipe.Create(doc, pipeSystemType.Id, pipeType.Id, newLevel.Id, Curve1.GetEndPoint(0), Curve1.GetEndPoint(1));                        
                       break;
                    default:
                        break;
                }

            }
            t.Commit();
            t.Dispose();

            return Result.Succeeded;
        }

        private WallType GetWallTypeByName(Document doc, string wallType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Walls);
            collector.WhereElementIsElementType();

            foreach (WallType glazingWT in collector)
            {
                if (glazingWT.Name == wallType)
                    return glazingWT;
            }
            return null;

        }
        private WallType GetWallTypeByName2(Document doc, string wallType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Walls);
            collector.WhereElementIsElementType();

            foreach (WallType solidWT in collector)
            {
                if (solidWT.Name == wallType)
                    return solidWT;
            }
            return null;

        }
        private PipeType GetPipeTypeByName(Document doc, string pipeType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));

            foreach (PipeType CurrentPT in collector)
            {
                if (CurrentPT.Name == pipeType)
                    return CurrentPT;
            }


            return null;
        }
        private DuctType GetDuctTypeByName(Document doc, string ductType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(DuctType));

            foreach (DuctType ductWT in collector)
            {
                if (ductWT.Name == ductType)
                    return ductWT;
            }
            return null;

        }   
        private MEPSystemType GetMEPSystemTypeByName(Document doc, string MEPtype)
        {
        FilteredElementCollector collector = new FilteredElementCollector(doc);
        collector.OfClass(typeof(MEPSystemType));

            foreach (MEPSystemType MEPwt in collector)
            {
                if (MEPwt.Name == MEPtype)
                    return MEPwt;
            }
                  return null;
        }
    }
 


      

    }
