   M S B A M L   `   `   `  ����I  EDeviceBatchWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]3clr-namespace:DeviceBatchWPF.Controls.BatchBuilding%DeviceBatchWPF.Controls.BatchBuilding  5      D    >DeviceBatchWPF.Controls.BatchBuilding.SelectBuildMethodControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Elocal3clr-namespace:DeviceBatchWPF.Controls.BatchBuilding     5      .��5	      �� $��Center=�6   $��Center=�6.   .+�5
   
   �� .��6   Who made this batch?6&   &   System.Windows.Controls.DataGrid  5   
   ��. $
��Auto=�5      $
��Auto��5          AutoGenerateColumns  .  $ �� IsSynchronizedWithCurrentItem$
 TrueY�5         CanUserAddRows .     SelectionMode$ Single=�5      ��5      �� *VisibleEmployees+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� SelectedItem 5      �� *SelectedEmployee+$ TwoWay=�$ PropertyChanged=�   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ First��6C   	   Width5  /System.Windows.Controls.DataGridLengthConverter$
	 Auto 6R   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 6%   �� *	FirstName+6a     5      $
 Last��6B   $
	 Auto 6P   
 6%   �� *
LastName+6_   5      �� 5   
   $��5��6   $	��200��6   $��Assign Selected Employee��6*   
����d���6j   �� *AssignSelectedEmployee+6�   �� 5   
   $��10q�6   .��6   Please choose:6,   � 5   
   .+�5      �� $��10q�5      
����d�$��Build from 
SpreadSheet��5       ��5      �� *ChooseToBuildFromSpreadsheet+5       �� 5"      $��10q�5#      
����d�$!��Build from 
DeviceTemplates��5&      ��5%      �� *!ChooseToBuildBatchFromTemplates+5&      �� 5(      $��10q�5)      
����d�$%��Build from previous
DeviceBatch��5,      ��5+      �� * ChooseToBuildBatchFromPrevious+5,      5.      5/      51      