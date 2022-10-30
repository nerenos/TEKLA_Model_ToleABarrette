using System;
using System.Collections;
using System.Collections.Generic;

using System.Windows.Forms;
using Tekla.Structures.Model;
using TSG3D = Tekla.Structures.Geometry3d;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model.Operations;


namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
		
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {	
			try
            {
				while (true)
				{
					//-------------- Variables --------------------
					double jeuini = 2;
					int htbarette = 7;
					int lgini = 50;
					int lgbarette = 30;
					int lgmax = 400;
					string myProfil = "PL2";
					double lF;
					double lC;
					
					double nX;
					double nY;
					double lX;
					double lY;
					double lginiX;
					double lginiY;
					
					
					Model myModel = new Model();
					myModel.CommitChanges();
					Picker myPicker = new Picker();
					Point myPoint1 = myPicker.PickPoint("Selectionnez le Point 1");
					Point myPoint2 = myPicker.PickPoint("Selectionnez le Point 2");
					Point myPoint3 = myPicker.PickPoint("Selectionnez le Point 3");
					
					Vector myVectorX = Vecteur(myPoint1, myPoint2);
					Vector myVectorY = Vecteur(myPoint1, myPoint3);
					
					double angle = myVectorX.GetAngleBetween(myVectorY);
					double jeu = jeuini/Math.Sin(angle);
					
					Point myPoint4 = new Point((myPoint2.X+myPoint3.X-1*myPoint1.X), (myPoint2.Y+myPoint3.Y-1*myPoint1.Y), (myPoint2.Z+myPoint3.Z-1*myPoint1.Z));
					
					ModelObject myPointOrigine = new ControlPoint(myPoint1);
					ModelObject myPointX = new ControlPoint(myPoint2);
					ModelObject myPointY = new ControlPoint(myPoint3);
					ModelObject myPointFin = new ControlPoint(myPoint4);
					
					myPointOrigine.Insert();
					myPointX.Insert();
					myPointY.Insert();
					myPointFin.Insert();
	

					ContourPlate CP = new ContourPlate();
					Chamfer myChamfer = new Chamfer();
					
					Point cP1 = Deplacement(myPoint1, myVectorX,myVectorY, jeu+htbarette, jeu+htbarette);
					
					Point cP2 = Deplacement(myPoint2, myVectorX, myVectorY, -jeu-htbarette, jeu+htbarette);
					Point cP3 = Deplacement(myPoint4, myVectorX, myVectorY, -jeu-htbarette, -jeu-htbarette);
					Point cP4 = Deplacement(myPoint3, myVectorX, myVectorY, jeu+htbarette, -jeu-htbarette);
					
					double lgX = Vecteur(cP1, cP2).GetLength();
					double lgY = Vecteur(cP2, cP3).GetLength();
					
					if (lgX<lgmax || lgX<(2*lgini+3*lgbarette))
					{
						lginiX = (lgX-lgbarette)/2;
						lX = lgX;
					}
					else
					{
						nX = Math.Floor((lgX-2*(lgini))/lgmax) + 1;
						lX = ((lgX-2*lgini-(nX+1)*lgbarette)/nX);
						lginiX = lgini;
					}
					
					if (lgY<lgmax || lgY<(2*lgini+3*lgbarette))
					{
						lginiY = (lgY-lgbarette)/2;
						lY = lgY;
					}
					else
					{
						nY = Math.Floor((lgY-2*(lgini))/lgmax) + 1;
						lY = ((lgY-2*lgini-(nY+1)*lgbarette)/nY);
						lginiY = lgini;
					}
					
				
					CP.AddContourPoint(new ContourPoint(cP1, myChamfer));
					Point tP = new Point();
					
					tP = Deplacement(cP1, myVectorX,myVectorY, lginiX, 0);
					lC = Vecteur(cP1, tP).GetLength();
					lF = Vecteur(cP1, cP2).GetLength();
					do {
						Point bP1 = tP;
						Point bP2 = Deplacement(tP, myVectorX, myVectorY, 0, -htbarette);
						Point bP3 = Deplacement(tP, myVectorX, myVectorY, lgbarette, -htbarette);
						Point bP4 = Deplacement(tP, myVectorX, myVectorY, lgbarette, 0);
						
						CP.AddContourPoint(new ContourPoint(bP1, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP2, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP3, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP4, myChamfer));
						tP = Deplacement(bP4, myVectorX,myVectorY, lX, 0);
						lC = Vecteur(cP1, tP).GetLength();
						
					}while(lC<lF);
										
					CP.AddContourPoint(new ContourPoint(cP2, myChamfer));
					
					tP = Deplacement(cP2, myVectorX,myVectorY, 0, lginiY);
					lC = Vecteur(cP2, tP).GetLength();
					lF = Vecteur(cP2, cP3).GetLength();
					do {
						Point bP1 = tP;
						Point bP2 = Deplacement(tP, myVectorX, myVectorY, htbarette, 0);
						Point bP3 = Deplacement(tP, myVectorX, myVectorY, htbarette, lgbarette);
						Point bP4 = Deplacement(tP, myVectorX, myVectorY, 0, lgbarette);
						
						CP.AddContourPoint(new ContourPoint(bP1, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP2, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP3, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP4, myChamfer));
						tP = Deplacement(bP4, myVectorX,myVectorY, 0, lY);
						lC = Vecteur(cP2, tP).GetLength();
						
					}while(lC<lF);
					
					CP.AddContourPoint(new ContourPoint(cP3, myChamfer));
					
					tP = Deplacement(cP3, myVectorX,myVectorY, -lginiX, 0);
					lC = Vecteur(cP3, tP).GetLength();
					lF = Vecteur(cP3, cP4).GetLength();
					do {
						Point bP1 = tP;
						Point bP2 = Deplacement(tP, myVectorX, myVectorY, 0, htbarette);
						Point bP3 = Deplacement(tP, myVectorX, myVectorY, -lgbarette, htbarette);
						Point bP4 = Deplacement(tP, myVectorX, myVectorY, -lgbarette, 0);
						
						CP.AddContourPoint(new ContourPoint(bP1, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP2, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP3, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP4, myChamfer));
						tP = Deplacement(bP4, myVectorX,myVectorY, -lX, 0);
						lC = Vecteur(cP3, tP).GetLength();
						
					}while(lC<lF);
					
					CP.AddContourPoint(new ContourPoint(cP4, myChamfer));
					
					tP = Deplacement(cP4, myVectorX,myVectorY, 0, -lginiY);
					lC = Vecteur(cP4, tP).GetLength();
					lF = Vecteur(cP4, cP1).GetLength();
					do {
						Point bP1 = tP;
						Point bP2 = Deplacement(tP, myVectorX, myVectorY, -htbarette, 0);
						Point bP3 = Deplacement(tP, myVectorX, myVectorY, -htbarette, -lgbarette);
						Point bP4 = Deplacement(tP, myVectorX, myVectorY, 0, -lgbarette);
						
						CP.AddContourPoint(new ContourPoint(bP1, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP2, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP3, myChamfer));
						CP.AddContourPoint(new ContourPoint(bP4, myChamfer));
						tP = Deplacement(bP4, myVectorX,myVectorY, 0, -lY);
						lC = Vecteur(cP4, tP).GetLength();
						
					}while(lC<lF);
					
					

					
					
					
					

					CP.Profile.ProfileString = myProfil;
					CP.Material.MaterialString = "S235JR";
					
					CP.Insert();
					
					

					
					//Point myPoint = new Point();
					ModelObject myPoints = new ControlPoint(Deplacement(cP1, myVectorX, myVectorY, lgini, 0));
					myPoints.Insert();
					
					
			
					

					
					
					
				}
			}
			catch
            {
                Operation.DisplayPrompt("Commande interrompue par l'utilisateur");
            }
		}
	
		public static Point Deplacement(Point myPoint, Vector myVectorX, Vector myVectorY, double coefX, double coefY)
		{
			double coefXm = coefX * Math.Sqrt(2);
			double coefYm = coefY * Math.Sqrt(2);
			double lengthX = myVectorX.GetLength();
			double lengthY = myVectorY.GetLength();
			Vector uVectorX = new Vector(myVectorX.X/lengthX, myVectorX.Y/lengthX, myVectorX.Z/lengthX);
			Vector uVectorY = new Vector(myVectorY.X/lengthY, myVectorY.Y/lengthY, myVectorY.Z/lengthY);
			return new Point(myPoint.X + coefX*uVectorX.X + coefY*uVectorY.X, myPoint.Y + coefX*uVectorX.Y + coefY*uVectorY.Y, myPoint.Z + coefX*uVectorX.Z + coefY*uVectorY.Z);
		}
		
		public static Vector Vecteur(Point ori, Point fin)
		{
			return new Vector(fin.X-ori.X, fin.Y-ori.Y, fin.Z-ori.Z);
		}
		
	}
}					
