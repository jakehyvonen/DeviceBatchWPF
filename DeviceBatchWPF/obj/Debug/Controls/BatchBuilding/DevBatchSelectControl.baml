   M S B A M L   `   `   `  ����I  EDeviceBatchWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]3clr-namespace:DeviceBatchWPF.Controls.BatchBuilding%DeviceBatchWPF.Controls.BatchBuilding  5      A    ;DeviceBatchWPF.Controls.BatchBuilding.DevBatchSelectControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Elocal3clr-namespace:DeviceBatchWPF.Controls.BatchBuilding     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,    � ColumnDefinitions 5   
   �� 5      $��* �6   6+   �� 5      $��* �6   6+   �� 5      $��* �6   6+   .+�5   
   (  "System.Windows.Controls.DatePicker  -   5        $��BeginDatePicker��5      $��0��6,   $��0��69      SelectedDate 6I   �� *BeginSearchDate+6t   �� 5   
   $��0��6   $��1��6"   .+�5      �� -   5        $��FilterButton��5      
����d�$��70��6%   ��60   �� *FilterDevBatches+.��6U   Filter Search6d   �� 5      -   5        $��SelectBatchButton��5      
��� E��$	��170��6*   ��66   �� *UseSelectedBatch+��6[   ��  �� ElementName$ lvDeviceBatches�� �� Path$ SelectedItem�.��6�   Use Selected Device Batch6�   5        5   
   -   5        $��EndDatePicker��5      $��0��6*   $��2��67    6G   �� *EndSearchDate+6p   �� 5   
   -   5        $��lvDeviceBatches��5   P   $��1��6   $��0��6    $��3��60   $��10q�6D   ��6g   �� *VisibleDeviceBatches+ �� UpdateSourceTrigger$ PropertyChanged=� �� View 5      �� 5      .P�5       �� $��
Batch Name��6%    �� DisplayMemberBinding 69   �� *Name+�� 5!      $��
Batch Size��6%    69   �� *Size+�� 5"      $��Fabrication Date��6%    6?   �� *	FabDate+�� 5#      $��	Directory��6%    68   �� *
FilePath+�� 5$      $��Fabrication Notes��6%    6@   �� *Notes+5%      5'      5(      5*      