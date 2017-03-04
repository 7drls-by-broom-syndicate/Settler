using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class RLMap {


    //
     // To get a Dijkstra map, you start with an integer array representing your map,
     // with some set of goal cells set to zero and all the rest set to a very high number.
     // Iterate through the map's "floor" cells -- skip the impassable wall cells.
     // If any floor tile has a value that is at least 2 greater than its lowest-value floor neighbor, 
     // set it to be exactly 1 greater than its lowest value neighbor. 
     // Repeat until no changes are made. 
     // The resulting grid of numbers represents the number of steps that it will take to get from any given tile to the nearest goal.
     // 



    //static const int dircount = 8;
	//static const int dxdy[dircount] = { 0, -1, 1, 0, 0, 1, -1, 0 };
    /*
    readonly int[] dxdy ={ 0, -1, 1, 0, 0, 1, -1, 0, 1, -1, 1, 1, -1, 1, -1, -1 };
    const int dircount = 16;

    bool PathfindDijkstra(int startx, int starty, int goalx, int goaly){

		Array2D<bool> visited=new Array2D<bool>(width, height,false);

		distance.Fill(int.MaxValueValue);

		int debugnodecount = 0;

		List<Cell> todolist=new List<Cell>();

		//int debugcounter = 0;

		//we start from the goal and try to find the start
		todolist.Add(new Cell( goalx, goaly ));

		//make the goal (actually the start) stand out. use distance
		distance[startx, starty] = -1;
		distance[goalx, goaly] = 0;

		while (todolist.Count>0){
			Cell t = todolist.OneFromTheTop();

			for (int f = 0; f < dircount; f += 2){
				int scanx = t.x + dxdy[f];
				int scany = t.y + dxdy[f + 1];

				if (scanx < 0 || scany < 0 || scanx >= width || scany >= height){
					//square is off the map
					continue;
				}
				if (visited[scanx, scany]){
					continue;
				}
				if (!passable[scanx, scany]){
					//not a walkable square
					visited[scanx, scany]= true;
					continue;
				}
				if (distance[scanx, scany] == -1){
					//found the goal (the start)

					


					distance[scanx, scany] = int.MaxValueValue - 1;
					int nodex = startx, nodey = starty;
					lastpath.Clear();

					int lowest = int.MaxValueValue;
					while (lowest != 0){
						int lowestx = int.MaxValueValue - 1, lowesty = int.MaxValueValue - 1;
						for (int nf = 0; nf < dircount; nf += 2){
							int nscanx = nodex + dxdy[nf];
							int nscany = nodey + dxdy[nf + 1];
							if (nscanx < 0 || nscany < 0 || nscanx >= width || nscany >= height)continue;
							if (distance[nscanx, nscany] < lowest){
								lowest = distance[nscanx, nscany];
								lowestx = nscanx; lowesty = nscany;
							}
						}
						lastpath.Add(new Cell( lowestx, lowesty ));
						nodex = lowestx; nodey = lowesty;
						//displaychar[lowestx, lowesty] = '*';
					}
					return true;
				}

				if (distance[scanx, scany] == int.MaxValueValue){
					todolist.Add(new Cell( scanx, scany ));
					//if (visited.at(scanx, scany)){
					//	std::cout << "1";
					//}
				}

				if (distance[t.x, t.y]+ 1 < distance[scanx, scany]){
					distance[scanx, scany] = distance[t.x, t.y] + 1;
					//displaychar.at(scanx, scany) = '.';// '0' + distance.at(t.x, t.y) + 1;
				}



			}
			visited[t.x, t.y]= true; debugnodecount++;

			//	debugcounter++;
			//	if (debugcounter == 25){
			//		std::cout << "begin" << std::endl;
			//		for (auto f : todolist){
			//			std::cout << distance.at(f.x, f.y) << std::endl;
			//		}
			//		std::cout << "end" << std::endl;
			//				DisplayOnConsole();
			//		debugcounter = 0;
			//	}
		}
		return false;
	}
    */


    readonly int[] dxdy = {0, -1, 1, 0, 0, 1, -1, 0, 1, -1, 1, 1, -1, 1, -1, -1 };

    public bool PathfindAStar(int startx, int starty, int goalx, int goaly, bool diags = true, bool fillpath = false,bool crosswater=false){

		int dircount = (diags) ? 16 : 8;
		
        //visited is our "closed list"
		Array2D<bool> visited=new Array2D<bool>(width, height,false);

		distance.Fill(int.MaxValue);

        var todolist = new Multimap<int, Cell>();

		//we start from the goal and try to find the start
		todolist.Insert( 0 + Distance_Euclidean(startx, starty, goalx, goaly),new Cell(goalx,goaly));//todolist.Insert(todopair{ 0 + Distance_Euclidean(startx, starty, goalx, goaly), { goalx, goaly } });

		//make the goal (actually the start) stand out. use distance
		distance[startx, starty] = -1;
		distance[goalx, goaly] = 0;

		while (!todolist.Empty()){
			Cell t = todolist.Begin();//crash- if click inaccessible maybe?

			for (int f = 0; f < dircount; f += 2){
				int scanx = t.x + dxdy[f];
				int scany = t.y + dxdy[f + 1];

				if (scanx < 0 || scany < 0 || scanx >= width || scany >= height){
					//square is off the map
					continue;
				}
				if (visited[scanx, scany]){
					continue;
				}
				if (!passable[scanx, scany]&&!(displaychar[scanx,scany]==Etilesprite.MAP_WATER&&crosswater)){
					//not a walkable square
					visited[scanx, scany]= true;
					continue;
				}
				if (distance[scanx, scany]== -1){
					//found the goal (the start)
									
					if (fillpath){
						distance[scanx, scany]= int.MaxValue - 1;
						int nodex = startx, nodey = starty;
						lastpath.Clear();

						int lowest = int.MaxValue;
						while (lowest != 0){
							int lowestx = int.MaxValue - 1, lowesty = int.MaxValue - 1;
							for (int ff = 0; ff < dircount; ff += 2){
								int sxf = nodex + dxdy[ff];
								int syf = nodey + dxdy[ff + 1];
								if (sxf < 0 || syf < 0 || sxf >= width || syf >= height)continue;
								if (distance[sxf, syf] < lowest){
									lowest = distance[sxf, syf];
									lowestx = sxf; lowesty = syf;
								}
							}
							lastpath.Enqueue(new Cell( lowestx, lowesty ));
							nodex = lowestx; nodey = lowesty;
						}//end while lowest !=0
					}//end if fill path
					else {
						int lowest = int.MaxValue;
						for (int ff = 0; ff < dircount; ff+=2){
							int sxf = startx + dxdy[ff];
							int syf = starty + dxdy[ff + 1];
							if (sxf < 0 || syf < 0 || sxf >= width || syf >= height)continue;
							if (distance[sxf, syf] < lowest){
								lowest = distance[sxf, syf];
								firststepx = sxf; firststepy = syf;
							}
						}				
					}
					return true;
				}//end found start

				bool sowasit = (distance[scanx, scany] == int.MaxValue);

				
				int cs_sxsy = Distance_Euclidean(scanx, scany, startx, starty);

				int d_txty = distance[t.x, t.y] + 1;
				int d_sxsy = distance[scanx, scany];
	
				if (sowasit || (d_txty < d_sxsy)){
					distance[scanx, scany] = d_txty;

				}

				if (sowasit){
					todolist.Insert(distance[scanx, scany] + cs_sxsy, new Cell(scanx, scany ) );
				}
			}

			visited[t.x, t.y]= true;
	
		}
		return false;


	}
}
