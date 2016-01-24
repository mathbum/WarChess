﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WarChess.Objects;
using WarChess.Objects.Items;

namespace Project1 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

		private Game Game;
		private List<List<Label>> labels;
		private Position SelectedPos = null;
		private List<Button> actionButtons = new List<Button>();
		private List<Rectangle> conflictRectangles = new List<Rectangle>();

		public MainWindow(Game Game) {
			InitializeComponent();
			this.Game = Game;
			int rows = this.Game.GetBoardRows();
			int cols = this.Game.GetBoardColumns();
			PlayerLabel.Content = this.Game.GetCurrentPlayer().Name;
			InitializeBoardGui(rows, cols);
			InitializePlacement();
		}

		private string LastSelectedUnitName;
		private Dictionary<Position,Unit> unitsPlaced = new Dictionary<Position, Unit>();
		private void InitializePlacement() {			
			EndTurnButton.Click -= EndTurn_Click;
			EndTurnButton.Click += Setup_EndTurn_Click;
			EndTurnButton.IsEnabled = true;
			EndTurnButton.Content = "Finalize Placements";
			SetUpPlacement();
		}
		private void SetUpPlacement() {
			RemoveUnit.IsEnabled = false;
			combo.Items.Clear();
			PointLimitLbl.Content = "0/" + Game.pointLimit;
			List<string> unitNames = Config.GetUnitNames(Config.Units);//this could also be a senario list of units
			for (int i = 0; i < unitNames.Count; i++) {
				combo.Items.Add(unitNames[i]);
			}
			combo.SelectedIndex = 0;
		}
		private void combo_SelectionChanged(object sender, SelectionChangedEventArgs e) {		
			if (combo.SelectedIndex!=-1) {
				LastSelectedUnitName = combo.SelectedItem.ToString();
			}
		}
		private void RemoveUnit_Click(object sender, RoutedEventArgs e) {
			unitsPlaced.Remove(SelectedPos);
			UpdatePoints();
			deselectUnit();
		}
		private void EquipmentList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			if(unitsPlaced.ContainsKey(SelectedPos)) {//its actual your unit (that you placed this turn)
				ListBox fromBox = (ListBox)sender;
				ListBox toBox = EquipableList;
				if(fromBox == EquipableList) {
					toBox = EquipList;
				}
				Unit unit = unitsPlaced[SelectedPos];
				SwitichItemStatus(fromBox, toBox, unit);
				UpdatePreview(unit);
				UpdatePoints();
			}
		}
		private void SwitichItemStatus(ListBox fromBox, ListBox toBox, Unit unit) {
			if (fromBox.SelectedIndex != -1) {
				Item item = (Item) fromBox.SelectedItem;
				fromBox.Items.Remove(item);
				toBox.Items.Add(item);
				if (toBox == EquipList) {
					int cost = Config.Units[unit.Name].CompatableItems[item];
					unit.EquipItems.Add(new KeyValuePair<Item, int>(item, cost));
				} else {
					unit.RemoveItemFromEquip(item);
				}				
			}
		}
		private bool SetUnit(Position position) {
			if (Game.IsValidPlacement(position)) {
				Unit unit = Game.CreateUnit(LastSelectedUnitName);
				unit.Position = position;
				unit.Player = Game.GetCurrentPlayer();
				unitsPlaced[position] = unit;
				displayPlacement();
				UpdatePoints();
				return true;
			}
			return false;			
		}
		private int CalculatePoints(List<Unit> units) {
			int points = 0;
			for(int i = 0; i < units.Count; i++) {
				points += units[i].GetPoints();
			}
			return points;
		}
		private void UpdatePoints() {
			int points = CalculatePoints(unitsPlaced.Values.ToList());
			PointLimitLbl.Content = points + "/" + Game.pointLimit;
			if (points <= Game.pointLimit) {
				EndTurnButton.IsEnabled = true;
				PointLimitLbl.Foreground = new SolidColorBrush(Colors.Black);
			} else {
				PointLimitLbl.Foreground = new SolidColorBrush(Colors.Red);
				EndTurnButton.IsEnabled = false;
			}
		}
		private void displayPlacement() {
			foreach(KeyValuePair<Position,Unit> kvp in unitsPlaced) {
				Label l = labels[kvp.Key.Row][kvp.Key.Column];
				l.Content = kvp.Value.Name;
				l.Background = new SolidColorBrush(Colors.Green);
			}
		}
		private void Setup_EndTurn_Click(object sender, RoutedEventArgs e) {
			Game.EndTurn();
			SetUpPlacement();
			SelectedPos = null;
			PlayerLabel.Content = Game.GetCurrentPlayer().Name;
			for(int i = 0; i < labels.Count; i++) {
				for(int j = 0; j < labels[i].Count; j++) {
					Label l = labels[i][j];
					l.Content = "";
					l.Background = null;
				}
			}
			List<Unit> units = unitsPlaced.Values.ToList();
			for(int i = 0; i < units.Count; i++) {
				Game.PlaceUnit(units[i].Position, units[i]);
			}
			EquipList.Items.Clear();
			EquipableList.Items.Clear();
			unitsPlaced.Clear();
			UpdateAllSquares();
			if (!Game.IsInSetup) {
				EndTurnButton.Click -= Setup_EndTurn_Click;
				EndTurnButton.Click += EndTurn_Click;
				PhaseLabel.Content = Game.Phase;
				MainGrid.Children.Remove(combo);
				MainGrid.Children.Remove(RemoveUnit);
				EquipableList.MouseDoubleClick -= EquipmentList_MouseDoubleClick;//TODO give it some ability to equip and unequip items
				EquipList.MouseDoubleClick -= EquipmentList_MouseDoubleClick;
				EndTurnButton.Content = "End Turn";				
			}
		}

		private void InitializeBoardGui(int rows, int cols) {
			int width = 75;
			int height = 75;
			grid.Width = cols * width;
			grid.Height = rows * height;
			grid.HorizontalAlignment = HorizontalAlignment.Left;
			grid.VerticalAlignment = VerticalAlignment.Top;
			grid.ShowGridLines = true;
			grid.Background = new SolidColorBrush(Colors.Blue);

			for (int i = 0; i < rows; i++) {
				RowDefinition gridRow1 = new RowDefinition();
				gridRow1.Height = new GridLength(height);
				grid.RowDefinitions.Add(gridRow1);
			}
			for (int i = 0; i < cols; i++) {
				ColumnDefinition gridCol1 = new ColumnDefinition();
				gridCol1.Width = new GridLength(width);
				grid.ColumnDefinitions.Add(gridCol1);
			}


			//for (int i = 0; i < rows; i++) {
			//	VirtualizingStackPanel v = new VirtualizingStackPanel();
			//	{
			//		v.Orientation = Orientation.Vertical;
			//		v.Width = 75;
			//	}
			//	for (int j = 0; j < cols; j++) {
			//		Image image = new Image();
			//		if (j == 3) {
			//			image.Source = Config.TerrainObjs['u'].Image;
			//		} else {
			//			image.Source = Config.TerrainObjs[' '].Image;//TODO make this more eff?									
			//		}
			//		v.Children.Add(image);
			//	}
			//	trythis.Children.Add(v);
			//}

			//grid.Visibility = Visibility.Collapsed;
			for (int i = 0; i < rows; i++) {
				for (int j = 0; j < cols; j++) {
					Image image = new Image();
					{
						image.Source = Game.GetTerrainAtPos(new Position(i, j)).Image;//TODO make this more eff?
						image.Stretch = Stretch.Fill;
					}
					Grid.SetRow(image, i);
					Grid.SetColumn(image, j);
					grid.Children.Add(image);
				}
			}

			labels = new List<List<Label>>();
			for (int i = 0; i < rows; i++) {
				List<Label> rowlabels = new List<Label>();
				for (int j = 0; j < cols; j++) {
					TextBlock positionTxtBlock = new TextBlock();
					{
						positionTxtBlock.Width = 30;
						positionTxtBlock.Height = 25;
						positionTxtBlock.Text = i.ToString() + ", " + j.ToString();
						positionTxtBlock.Foreground = new SolidColorBrush(Colors.White);
						positionTxtBlock.VerticalAlignment = VerticalAlignment.Top;
						positionTxtBlock.HorizontalAlignment = HorizontalAlignment.Left;
					}
					Grid.SetRow(positionTxtBlock, i);
					Grid.SetColumn(positionTxtBlock, j);
					grid.Children.Add(positionTxtBlock);

					Label l2 = CreateLabel(40, 25, "", grid, i, j);
					rowlabels.Add(l2);
				}
				labels.Add(rowlabels);
			}
			UpdateAllSquares();
		}
		public Button CreateButton(int width, int height, string content,RoutedEventHandler ClickEvent, Grid grid = null,int row = -1, int column = -1) {
			Button b = new Button(); {			
				b.Width = width;
				b.Height = height;
				b.Foreground = new SolidColorBrush(Colors.Black);
				b.Content = content;
				b.FontSize = 10;
				b.VerticalAlignment = VerticalAlignment.Center;
				b.HorizontalAlignment = HorizontalAlignment.Center;
				b.VerticalContentAlignment = VerticalAlignment.Center;
				b.HorizontalContentAlignment = HorizontalAlignment.Center;
			};
			b.Click += ClickEvent;
			if(grid!=null && row!=-1 && column != -1) {
				Grid.SetRow(b, row);
				Grid.SetColumn(b, column);
				grid.Children.Add(b);
			}
			return b;
		}
		public Label CreateLabel(int width, int height, string content, Grid grid = null, int row = -1, int column = -1) {
			Label lbl = new Label();
			{
				lbl.Width = width;
				lbl.Height = height;
				lbl.Foreground = new SolidColorBrush(Colors.White);
				lbl.Content = content;
				lbl.FontSize = 10;
				lbl.VerticalAlignment = VerticalAlignment.Center;
				lbl.HorizontalAlignment = HorizontalAlignment.Center;
				lbl.VerticalContentAlignment = VerticalAlignment.Center;
				lbl.HorizontalContentAlignment = HorizontalAlignment.Center;
			}
			if (grid != null && row != -1 && column != -1) {
				Grid.SetRow(lbl, row);
				Grid.SetColumn(lbl, column);
				grid.Children.Add(lbl);
			}
			return lbl;
		}

		private Position GetPosOfClickedCell() {
			var point = Mouse.GetPosition(grid);

			int row = 0;
			int col = 0;
			double accumulatedHeight = 0.0;
			double accumulatedWidth = 0.0;

			// calc row mouse was over
			foreach (var rowDefinition in grid.RowDefinitions) {
				accumulatedHeight += rowDefinition.ActualHeight;
				if (accumulatedHeight >= point.Y)
					break;
				row++;
			}

			// calc col mouse was over
			foreach (var columnDefinition in grid.ColumnDefinitions) {
				accumulatedWidth += columnDefinition.ActualWidth;
				if (accumulatedWidth >= point.X)
					break;
				col++;
			}
			return new Position(row, col);
		}
		private void selectUnit(Unit unit) {
			Position position = unit.Position;
			SelectedPos = position;
			UpdateLeftPane(unit);
			labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);
			if(unit.Player == Game.GetCurrentPlayer()) {
				if(Game.IsInSetup) {
					if(unit.Player == Game.GetCurrentPlayer()) {
						RemoveUnit.IsEnabled = true;
					}
				} else if(Game.Phase == Game.Phases.Move) {//based upon phase, display gui options
					DisplayGuiOptions();
				} else if(Game.Phase == Game.Phases.Shoot) {
					ShowShotOptions();
				}
			}
		}
		private void deselectUnit() {
			Unit unit = Game.GetUnitAtPos(SelectedPos);
			if(Game.IsInSetup) {
				RemoveUnit.IsEnabled = false;
				if(unitsPlaced.ContainsKey(SelectedPos)) {//if you are deselecting your unit
					unit = unitsPlaced[SelectedPos];
				}
			} else if(Game.Phase == Game.Phases.Move) {//based upon phase remove gui stuff
				RemoveGuiOptions();
			} else if(Game.Phase == Game.Phases.Shoot) {
				RemoveGuiOptions();//shouldn't go through code to removemoveoptions
			}
			UpdateSquare(SelectedPos, unit, true);
			SelectedPos = null;
			UpdateLeftPane(Config.NullUnit);
		}
		private void setUnit(Position position, Unit unit) {
			if(!SetUnit(position)) {
				return;//failed to place a unit
			}
			unit = unitsPlaced[position];
			selectUnit(unit);
		}		
		private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			//if (e.ClickCount == 2) { // for double-click, remove this condition if only want single click
			//MessageBox.Show(string.Format("Grid clicked at row {0}, column {1}", row, col));
			// row and col now correspond Grid's RowDefinition and ColumnDefinition mouse was over when double clicked!
			if(!(e.Source is Button)) {//only continue if the player didn't click a button
				Position position = GetPosOfClickedCell();
				Unit unit = Game.GetUnitAtPos(position);

				if(Game.IsInSetup && unitsPlaced.ContainsKey(position)) {
					unit = unitsPlaced[position];
				}
				if(SelectedPos == null) {//placing a unit or selecting a unit
					if(Game.IsInSetup && unit == Config.NullUnit) {//placing unit
						setUnit(position, unit);
					} else if(unit != Config.NullUnit) {//selecting unit
						selectUnit(unit);
					}							
				} else {//had someone else previously selected, deselecting/changing selection/perfmove
					if(SelectedPos.Equals(position)) {//deslect unit
						deselectUnit();
					} else if(unit != Config.NullUnit) {//changing selection
						deselectUnit();
						selectUnit(unit);
					} else {//clicked on null unit
						if(Game.GetUnitAtPos(SelectedPos).Player == Game.GetCurrentPlayer() && Game.Phase == Game.Phases.Move && !Game.IsInSetup) {
							perfmove(position);
						}else if(Game.IsInSetup) {//placing unit
							deselectUnit();
							setUnit(position, unit);
						}
					}
				}
			}

			
			//if(SelectedPos!=null && SelectedPos.Equals(position)) {//deselectingunit
			//	DeselectUnit();
			//}else if (unit.Player == Game.GetCurrentPlayer()) {//selecting your unit
			//	if (SelectedPos != null) {//had a previous unit selected			
			//		if (Game.IsInSetup) {//based upon phase remove gui options
			//			labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
			//		} else {
			//			DeselectUnit();
			//		}
			//	}
			//	UpdateLeftPane(unit);
			//	SelectedPos = position;
			//	labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);
			//	if (Game.IsInSetup) {
			//		SetupSelect(position, unit);
			//	} else if (Game.Phase == Game.Phases.Move) {//based upon phase, display gui options
			//		DisplayGuiOptions();
			//	} else if (Game.Phase == Game.Phases.Shoot) {
			//		ShowShotOptions();
			//	}				
			//} else if (SelectedPos != null && !Game.IsInSetup) {
			//	if (Game.Phase == Game.Phases.Move) {//if in move phase then move unit
			//		perfmove(position);
			//		UpdateLeftPane(unit);
			//	}			
			//}else if(Game.IsInSetup) {//if you are not deselecting, seleting another friendly unit or moveing a unit (only for movement phase) then you are placing a unit				
			//	UpdateLeftPane(unit);
			//	SetupSelect(position, unit);
			//}else if (unit != Config.NullUnit && unit.Player != Game.GetCurrentPlayer()) {
			//	UpdateLeftPane(unit);
			//}



			////TODO only show preview for non null units
			////if (Game.IsInSetup) {
			////	if (LastSelectedUnitName != null) {
			////		HandleClick(position);
			////	}
			////} else {			
			////	if (Game.Phase == Game.Phases.Move) {					
			////		if (SelectedPos != null && SelectedPos.Equals(position)) {//deslecting
			////			labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
			////			SelectedPos = null;
			////			RemoveGuiOptions();
			////		} else if (Game.GetUnitAtPos(position).Player == Game.GetCurrentPlayer()) {//selecting your unit
			////			if (SelectedPos != null) {//previously had a different unit selected
			////				labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
			////				RemoveGuiOptions();
			////			}
			////			SelectedPos = position;
			////			labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);
			////			DisplayGuiOptions();
			////		} else if (SelectedPos != null) {//moving your unit
			////			perfmove(position);//TODO trying to move a unit to a null position because it is supposed to be a jump.
			////		}
			////	} else if (Game.Phase == Game.Phases.Shoot) {
			////		if (SelectedPos != null && SelectedPos.Equals(position)) { //descecting your unit
			////			SelectedPos = null;
			////			RemoveGuiOptions();//shouldn't go through code to removemoveoptions
			////		} else if (Game.GetUnitAtPos(position).Player == Game.GetCurrentPlayer()) {//selecting your unit
			////			if (SelectedPos != null) {//previously had a different unit selected
			////				labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Green);
			////				RemoveGuiOptions();
			////			}
			////			SelectedPos = position;
			////			labels[position.Row][position.Column].Background = new SolidColorBrush(Colors.Black);

			////			ShowShotOptions();
			////		} else if (SelectedPos != null) {
			////			//UpdateAllSquares();//no
			////			//ShowShot(position);
			////		}
			////	}
			////}
		}
		private void ShowShotOptions() {
			List<Position> ShotOptions = Game.GetShotOptions(SelectedPos);
			for (int i = 0; i < ShotOptions.Count; i++) {
				Position pos = ShotOptions[i];
				Button b = CreateButton(40, 35, "Shoot", ShootTarget, grid, pos.Row, pos.Column);
				actionButtons.Add(b);
			}
		}
		private void ShootTarget(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position position = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			Game.Shoot(SelectedPos, position);
			RemoveGuiOptions();//shouldn't go through code to removemoveoptions?			
		}
		private void ShowShot(Position Target) {
			List<List<Position>> ShotPathDetails = Game.GetShotPathDetails(SelectedPos,Target);//should always be len 3, good shot pos, iffy shot pos, bad shot pos
			Position pos;
			Color color;
			for(int i = 0; i < ShotPathDetails.Count; i++) {
				for(int j = 0; j < ShotPathDetails[i].Count; j++) {
					pos = ShotPathDetails[i][j];
					if (i == 0) {
						color = Colors.Blue;
					}else if (i == 1) {
						color = Colors.Yellow;
					}else{
						color = Colors.Violet;
					}
					labels[pos.Row][pos.Column].Background = new SolidColorBrush(color);
					labels[pos.Row][pos.Column].Content = SelectedPos.Distance(pos);
				}
			}
		}
		private void perfmove(Position position) {
			bool succ = Game.Move(SelectedPos, position);
			if (succ) {
				Unit unit = Game.GetUnitAtPos(position);
				UpdateSquare(SelectedPos, Config.NullUnit, false);//because behind him must be a null unit
				//UpdateSquare(position, Game.GetUnitAtPos(position), false);
				RemoveGuiOptions();
				selectUnit(unit);
			}
		}
		private void DisplayJumpOptions(Position position) {
			List<Position> JumpOptions = Game.GetJumpablePos(position);
			for (int i = 0; i < JumpOptions.Count; i++) {//positions you can jump
				actionButtons.Add(CreateButton(40, 35, "Jump", JumpTerrain, grid, JumpOptions[i].Row, JumpOptions[i].Column));
			}
		}
		private void RemoveGuiOptions() {
			RemoveActionOptions();
			RemoveMoveOptions();
		}
		private void DisplayGuiOptions() {
			if (SelectedPos != null) {
				DisplayChargeOptions(SelectedPos);
				DisplayMoveOptions(SelectedPos);
				DisplayJumpOptions(SelectedPos);
			}
		}
		private void JumpTerrain(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position position = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			SelectedPos = Game.Jump(Game.GetUnitAtPos(SelectedPos),position);
			RemoveGuiOptions();
			if (SelectedPos != null) {
				selectUnit(Game.GetUnitAtPos(SelectedPos));
				//DisplayGuiOptions();
				//labels[SelectedPos.Row][SelectedPos.Column].Background = new SolidColorBrush(Colors.Black);
			}//else unit has died
		}
		private void RemoveMoveOptions() {
			UpdateAllSquares();
		}
		private void DisplayMoveOptions(Position position) {
			List<KeyValuePair<Position,int>> moves = Game.GetMoves(position);
			for (int i = 0; i < moves.Count; i++) {
				Position movePosition = moves[i].Key;
				if (movePosition.Equals(position)) {
					continue;
				}
				int cost = moves[i].Value;				
				Color color = Colors.Violet;//nullish shouldn't ever be this
				if (cost == 0) {
					color = Colors.White;//don't hardcode thses
				}else if (cost == 1) {
					color = Colors.GreenYellow;
				}else if (cost == 2) {
					color = Colors.Orange;
				}else if (cost == 3) {
					color = Colors.OrangeRed;
				}
				labels[movePosition.Row][movePosition.Column].Background = new SolidColorBrush(color);
			}
		}
		private int DisplayResolveOptions() {
			List<Unit> defenders = Game.GetConfictDefenders();
			for(int i = 0; i < defenders.Count; i++) {
				Unit defender = defenders[i];
				actionButtons.Add(CreateButton(40, 35, "Resolve", ResolveConflict, grid, defender.Position.Row, defender.Position.Column));
			}
			return defenders.Count;
		}
		private void ResolveConflict(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position defendingPosition = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			List<Unit> Strikable = Game.ResolveConflict(defendingPosition);
			if(Strikable.Count == 0) {//conflict autoresolved
				ResetGui();
				//PlayerLabel.Content = Game.GetCurrentPlayer().Name;//TODO this has duplicate code with endturn
				//PhaseLabel.Content = Game.Phase;
				//RemoveActionOptions();//this also updates all squares
				//SelectedPos = null;
				//DisplayTempConflicts();				
				//if(Game.Phase == Game.Phases.Move) {
				//	EndTurnButton.IsEnabled = true;
				//	UpdateAllSquares();
				//} else {
				//	DisplayResolveOptions();
				//}
				//might be next round
			} else{//defender has won and has to choose targets
				RemoveActionOptions();
				ShowStrikeOptions(Strikable);
				//update current player, and updateallsquares
				PlayerLabel.Content = Game.GetCurrentPlayer().Name;
				UpdateAllSquares();
			}
		}
		private void ShowStrikeOptions(List<Unit> possibleTargets) {
			for(int i = 0; i < possibleTargets.Count; i++) {
				Unit defender = possibleTargets[i];
				actionButtons.Add(CreateButton(40, 35, "Strike", StrikeUnit, grid, defender.Position.Row, defender.Position.Column));
			}
		}
		private void StrikeUnit(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position defendingPosition = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			List<Unit> possibleTargets = Game.ResolveStrike(defendingPosition);
			if(possibleTargets.Count==0) {//if the conflict is over				
				ResetGui();
				//PlayerLabel.Content = Game.GetCurrentPlayer().Name;//TODO this has duplicate code with endturn
				//PhaseLabel.Content = Game.Phase;
				//RemoveActionOptions();
				//UpdateAllSquares();
				//SelectedPos = null;
				//DisplayTempConflicts();

				//if(Game.Phase == Game.Phases.Move) {
				//	EndTurnButton.IsEnabled = true;
				//}
			} else {
				DisplayTempConflicts();
				UpdateAllSquares();
				RemoveActionOptions();
				ShowStrikeOptions(possibleTargets);
			}
		}
		private void RemoveActionOptions() {
			for(int i = 0; i < actionButtons.Count; i++) {
				grid.Children.Remove(actionButtons[i]);
			}
			actionButtons.Clear();
		}
		private void DisplayChargeOptions(Position position) {
			List<List<Position>> positions = Game.GetPossibleAttackPos(position);//should only ever be 2 lists in possibleattacks
			List<Position> positionsattacked = positions[0];
			List<Position> attackablepositions = positions[1];
			for (int i = 0; i < positionsattacked.Count; i++) {//positions you attacked earlier this turn
				actionButtons.Add(CreateButton(40, 35, "Cancel", CancelCharge, grid, positionsattacked[i].Row, positionsattacked[i].Column));
			}
			for (int i = 0; i < attackablepositions.Count; i++) {//positions you can attack
				actionButtons.Add(CreateButton(40, 35, "Attack", ChargeUnit, grid, attackablepositions[i].Row, attackablepositions[i].Column));
			}
		}
		private void ChargeUnit(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position defendingPosition = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			Game.AddCharge(Game.GetUnitAtPos(defendingPosition), Game.GetUnitAtPos(SelectedPos));
			RemoveGuiOptions();
			DisplayChargeOptions(SelectedPos);//can't jump or move
			DisplayTempConflicts();
		}
		private void CancelCharge(object sender, RoutedEventArgs e) {
			Button b = (Button)sender;
			Position defendingPosition = new Position(Grid.GetRow(b), Grid.GetColumn(b));
			Game.RemoveCharge(Game.GetUnitAtPos(defendingPosition), Game.GetUnitAtPos(SelectedPos));
			RemoveGuiOptions();
			DisplayGuiOptions();
			DisplayTempConflicts();
		}

		private void UpdatePreview(Unit unit) {			
			Namelabel.Content = unit.Name;
			Pointslabellbl.Content = unit.GetPoints();
			Strengthlabellbl.Content = unit.Strength;
			Defenselabellbl.Content = unit.GetDefense();
			Attackslbl.Content = unit.Attacks;
			Woundslbl.Content = unit.Wounds;
			if (unit == Config.NullUnit || unit == null) {
				UnitPlayerLbl.Content = "None";
			} else {
				UnitPlayerLbl.Content = unit.Player.Name;
			}
			SolidColorBrush color = new SolidColorBrush(Colors.Red);
			string Conflictlbl = "In Conflict";//TODO don't hardcode this
			if (!unit.InConflict) {
				Conflictlbl = "Not In Conflict";
				color = new SolidColorBrush(Colors.Green);
			}
			InConflictLbl.Content = Conflictlbl;
			InConflictLbl.Background = color;
		}
		private void UpdateLeftPane(Unit unit) {
			EquipableList.Items.Clear();
			EquipList.Items.Clear();
			if (unit != Config.NullUnit) {				
				List<KeyValuePair<Item, int>> EquipItems = unit.EquipItems;
				for (int i = 0; i < EquipItems.Count; i++) {
					EquipList.Items.Add(EquipItems[i].Key);
				}
				if (!Game.IsInSetup) {
					List<KeyValuePair<Item, int>> UnequipItems = unit.UnequipItems;
					for (int i = 0; i < UnequipItems.Count; i++) {
						EquipableList.Items.Add(UnequipItems[i].Key);
					}
				} else {
					List<KeyValuePair<Item, int>> CompatableItems = Config.Units[unit.Name].CompatableItems.ToList();
					for (int i = 0; i < CompatableItems.Count; i++) {
						Item item = CompatableItems[i].Key;
						if (!unit.HasItem(item)) {
							EquipableList.Items.Add(item);
						}
					}
				}							
			}
			UpdatePreview(unit);
		}

		private void UpdateSquare(Position position,Unit unit, bool IsDeselect) {
			Label labelatpos = labels[position.Row][position.Column];
			labelatpos.Content = unit.Name;

			if(unit == Config.NullUnit) {
				labelatpos.Background = null;
			} else if(SelectedPos != null && SelectedPos.Equals(position) && !IsDeselect) {
				labelatpos.Background = new SolidColorBrush(Colors.Black);
			}else if(unit.Player == Game.GetCurrentPlayer()) {
				labelatpos.Background = new SolidColorBrush(Colors.Green);
			} else {
				labelatpos.Background = new SolidColorBrush(Colors.Red);
			}
		}

		private void UpdateAllSquares() {
			Position position = new Position();
			for(int i = 0; i < labels.Count; i++) {
				position.Row = i;
				for (int j = 0; j < labels[i].Count; j++) {
					position.Column = j;
					UpdateSquare(position, Game.GetUnitAtPos(position), false);
				}
			}
		}

		private void EndTurn_Click(object sender, RoutedEventArgs e) {
			Game.EndTurn();
			ResetGui();
		}
		private void ResetGui() {
			PlayerLabel.Content = Game.GetCurrentPlayer().Name;
			PhaseLabel.Content = Game.Phase;
			SelectedPos = null;
			RemoveGuiOptions();//this also updates all squares			
			DisplayTempConflicts(); //TODO remove this. It is only here to remove tempconflicts at the begining of a new round			
			if(Game.Phase == Game.Phases.Fight) {
				if(DisplayResolveOptions() > 0) {//if there are conflicts
					EndTurnButton.IsEnabled = false;
				} else {
					Game.EndTurn();//if there are no conflict then jump past fight phase
					PlayerLabel.Content = Game.GetCurrentPlayer().Name;
					PhaseLabel.Content = Game.Phase;
					UpdateAllSquares();//in case priority changes				
				}
			} else {
				EndTurnButton.IsEnabled = true;//this gets called more than necesarry
			}
		}

		/// <summary>
		/// ////////////////////////////////////////////
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		private void ClearConflictsDisplay() {
			for(int i = 0; i < conflictRectangles.Count; i++) {
				grid.Children.Remove(conflictRectangles[i]);
			}
			conflictRectangles.Clear();
		}
		private void DisplayTempConflicts() {
			ClearConflictsDisplay();
			foreach(KeyValuePair<Unit,List<Unit>> conflictItem in Game.GetConflicts()) { 
				Position defenderpos = conflictItem.Key.Position;
				for (int j = 0; j < conflictItem.Value.Count; j++) {					
					Rectangle rect = new Rectangle();
					{
						rect.Stroke = new SolidColorBrush(Colors.Black);
						rect.Margin = new Thickness(10, 15, 10, 15);
					}
					Position attackerpos = conflictItem.Value[j].Position;
					
					if (defenderpos.Row > attackerpos.Row) {//attacker is above
						rect.SetValue(Grid.RowSpanProperty, 2);
						Grid.SetRow(rect, attackerpos.Row);
						Grid.SetColumn(rect, attackerpos.Column);
					} else if (defenderpos.Column < attackerpos.Column) {//attacker is to the right
						rect.SetValue(Grid.ColumnSpanProperty, 2);
						Grid.SetRow(rect, defenderpos.Row);
						Grid.SetColumn(rect, defenderpos.Column);
					} else if (defenderpos.Row < attackerpos.Row) {//attacker is below
						rect.SetValue(Grid.RowSpanProperty, 2);
						Grid.SetRow(rect, defenderpos.Row);
						Grid.SetColumn(rect, defenderpos.Column);
					} else if (defenderpos.Column > attackerpos.Column) {//attacker is to the left
						rect.SetValue(Grid.ColumnSpanProperty, 2);
						Grid.SetRow(rect, attackerpos.Row);
						Grid.SetColumn(rect, attackerpos.Column);
					}
					grid.Children.Add(rect);
					conflictRectangles.Add(rect);
				}
			}							
		}
		private Position lastHover = null;
		private void grid_MouseMove(object sender, MouseEventArgs e) {//want to also be able to check shots in the movement phase. 
			if (Game.Phase == Game.Phases.Shoot) {
				if (SelectedPos != null) {
					Position pos = GetPosOfClickedCell();
					if ((lastHover == null || !lastHover.Equals(pos)) && !SelectedPos.Equals(pos)) {//also want to see if i have a good shot on my target but its covered by button
						lastHover = pos;
						UpdateAllSquares();//to remove last hover colors
						ShowShot(pos);
					}
				}
			}
		}		
		/////////////////////////////////////////////////////////////////
	}
}