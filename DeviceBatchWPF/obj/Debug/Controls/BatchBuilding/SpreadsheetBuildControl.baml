   M S B A M L   `   `   `  ����I  EDeviceBatchWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]3clr-namespace:DeviceBatchWPF.Controls.BatchBuilding%DeviceBatchWPF.Controls.BatchBuilding  5      C    =DeviceBatchWPF.Controls.BatchBuilding.SpreadsheetBuildControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Elocal3clr-namespace:DeviceBatchWPF.Controls.BatchBuilding     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#   �� 5      $��* �6#   .+�5      �� $��0��6   .+�5      �� $��Center=�6   $
��Left=�65   $	��250��6P    �� TextWrapping$ WrapWithOverflow=�6\   {�.  ��6|   �� *" DeviceBatchToAdd.SpreadSheetPath+ �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5      $��Center=�6   $
��Left=�64   $	��250��6O   
�����F���6[   �� *SelectSpreadSheet+.��6�   Select Spreadsheet6�   5      �� 5      $��1��6   .+�5      �� $
��Bold"�6   .��6*   Fabrication Date6<   (  "System.Windows.Controls.DatePicker  5         SelectedDate 6   �� *DeviceBatchToAdd.FabDate+$ PropertyChanged=�6u   5      �� 5      $��2��6   .+�5      �� $��Center=�6   $
��Left=�64   $	��250��6O   
��� E����6[   �� * SaveDeviceBatchFromSpreadsheet+.��6�   Save Device Batch6�   5      5       &   System.Windows.Controls.DataGrid  5!   
   $��1��5#      ��5"      �� *DeviceBatchToAdd.Devices+$ PropertyChanged=�   Columns 5$      4  .System.Windows.Controls.DataGridTemplateColumn  5%      ,  &System.Windows.Controls.DataGridColumn   Header$ Index��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 68      SortMemberPath$ 
BatchIndex��6E   	   SortDirection$	 
DescendingX�6a   
   CellTemplate
 5&      �� 5'      .R�5(      �� ��6(   �� *
BatchIndex+6F   5)      5+        5,      $ Layers��6)   $
 Auto 69   
 5-      �� 5.      deviceLayersTemplate��.R�5/      �� $��0q�6'   ��62   �� *Layers+$ PropertyChanged=���50   "   �� 51   &   .R�52   *   �  � Orientation$ 
Horizontal=�64    K� IsItemsHost . 6M   53   '   ��55   "   �� 56   &    �� 
TargetType" ��6,   .�57   *   ��  �� Property � �� �� Value
 �����6H   �� 58   *    � ��$ 1q�6L   59   '   ��5;   "   �� 5<   &   .R�5=   *   �� $��77��5>   -   $
 Wrap=�5?   -   ��5@   -   �� *SpreadSheetCellText+$ PropertyChanged=� �� Mode$ TwoWay=�5B   '   5D      5E      5G        5H      $ 
Data Label��6)   $
 Auto 6=   
 5I      �� 5J      .R�5K      �� ��6&   �� *Label+$ PropertyChanged=�$ TwoWay=�6q   5L      5N      5P      5Q      5S      