   M S B A M L   `   `   `  ����I  EDeviceBatchWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]3clr-namespace:DeviceBatchWPF.Controls.BatchBuilding%DeviceBatchWPF.Controls.BatchBuilding  5      G    ADeviceBatchWPF.Controls.BatchBuilding.BatchMaterialsSelectControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Elocal3clr-namespace:DeviceBatchWPF.Controls.BatchBuilding     5      .��5
      �   � RowDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   � $��0��6   
������� � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   �� 5      $��* �6#   .+�5      � $��0��6   .+�5      �� $
��Bold"�6   .��6*   Fabrication Date:6=   (  "System.Windows.Controls.DatePicker  5         SelectedDate 6   �� *DeviceBatchToAdd.FabDate+6Q   5      �� 5      $��3��5      $��Right=�5      $	��200��5      $+��%Assign Materials/Solutions to devices��5       
��� E����5"      �� *" GoToAssignMaterialsAndProperties+5#      5$      �� 5%   
   $��1��6   $��33��6!   6/   � 5&   
   
�������$��2��5(        5*      �� 5+      $
��Auto �6    �� 5,      $��* �6     5.      �� 5/      $��* �6#   �� 50      $��* �6#   .+�52      � $��0��6   $��0��6     53      �� 54      $��* �6'   �� 55      $��* �6'   .+�57      �� $��0��6   $
��Bold"�6(   .��6:   .,Solution-processible Materials In This Batch6h   �� 58      $��1��59      $��Right=�5:      $��Remove Selected Solution��5;      
����d���5=      �� *DeleteSelectedSolution+5>      5?      &   System.Windows.Controls.DataGrid  5@      $��1��5A      $��0��5B         AutoGenerateColumns .     SelectionMode$ Single=�5F         CanUserAddRows .  ��5C      �� *SolutionsToAdd+ �� UpdateSourceTrigger$ PropertyChanged=� �� SelectedItem 5E      �� *SelectedSolution+ J� InputBindings 5I      �� 5J      
	 �� Key$	 Space��6!   
 �� Command
 6-   �� *AddSelectedSolution+6W      Columns 5L      0  *System.Windows.Controls.DataGridTextColumn  5M      ,  &System.Windows.Controls.DataGridColumn   Header$ Label��6C   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6)   �� *Label+6T     5N      $ Concentration
(mg/mL)��6K    6)   �� *Concentration+6q     5O      $ Date Prepared��6k    6)   �� *DatePreparedOrPurified+ �� StringFormat$ MM/dd/yy��6�     5P      $ Solvent��6E    6)   �� *	Solvent+6X   5R      � 5S      $��0��5T      $��1��5U       5W      �� 5X      $��* �6'   �� 5Y      $��* �6'   .+�5[      �� $��0��6   $
��Bold"�6(   .��6:   Other Materials In This Batch6Y   �� 5\      $��1��5]      $	��200��5^      $��Right=�5_      $��Remove Selected Material��5`      
����d���5b      �� *DeleteSelectedMaterial+5c      5d        5e      $��1��6   $��1��5f       .  $ Single=�5j      ��5g      �� *MaterialsToAdd+$ PropertyChanged=� 5i      �� *SelectedMaterial+ 5k        5l      $
 Name��6B    6)   �� *Name+6R   5o      5p      �� 5q   
   $��3��6   .+�5r      �� .��6   Filter by role:6%   �� 5s      
����d� �� DisplayMemberPath$ LongName��5u      ��5v      �� *PhysicalRolesCollection+$ PropertyChanged=� 5w      �� *SelectedRole+$ PropertyChanged=� �� Mode$ TwoWay=�5y      � 5z   
   $��4��5{        5}      �� 5~      $
��Auto �6    �� 5      $��* �6     5�      �� 5�      $��* �6#   �� 5�      $��* �6#   .+�5�      � $��0��6   $��0��6     5�      �� 5�      $��* �6'   �� 5�      $��* �6'   .+�5�      �� $
��Bold"�6   $��0��6*   .��6:   *(Available Solution-processible Materials6d   �� 5�      $��1��6   $��Right=�6)   $	��200��6E   $��Add Selected Solution��6Q   
����d���6�   �� *AddSelectedSolution+6�   5�        5�      $��1��5�      $��0��5�      $	��333��5�      ��. $
��Auto=�5�       .   .  $ Single=�5�      ��5�      �� *SolutionsView+$ PropertyChanged=� 5�      �� *SelectedSolution+ 5�        5�      $ Material��6K    6)   �� *Material.Name+6_     5�      $ Label��6C    6)   �� *Label+6T     5�      $ Concentration
(mg/mL)��6K    6)   �� *Concentration+6q     5�      $ Date Prepared��6k    6)   �� *DatePreparedOrPurified+$ MM/dd/yy��6�     5�      $ Solvent��6E    6)   �� *	Solvent+6X   5�      � 5�      $��0��6   $��1��6     5�      �� 5�      $��* �6'   �� 5�      $��* �6'   .+�5�      �� $
��Bold"�6   $��0��6*   .��6:   Other Available Materials6U   �� 5�      $��1��5�      $��Right=�5�      $	��200��5�      $��Add Selected Material��5�      
����d���5�      �� *AddSelectedMaterial+5�      5�        5�      $��1��5�      $��1��5�      $	��333��5�       .  $ Single=�5�      ��5�      �� *MaterialsView+$ PropertyChanged=� 5�      �� *SelectedMaterial+ 5�        5�      $
 Name��6B    6)   �� *Name+6R   5�      5�      5�      5�      