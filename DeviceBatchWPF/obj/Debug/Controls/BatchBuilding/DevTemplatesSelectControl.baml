   M S B A M L   `   `   `  ����N  JDeviceBatchGenerics, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullfFclr-namespace:DeviceBatchGenerics.Support;assembly=DeviceBatchGenericsDeviceBatchGenerics.Support  5       DeviceBatchWPFE   ?DeviceBatchWPF.Controls.BatchBuilding.DevTemplatesSelectControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      UdbFclr-namespace:DeviceBatchGenerics.Support;assembly=DeviceBatchGenerics     5        � 	Resources  5
      7   1DeviceBatchGenerics.Support.LayerTemplateSelector   layerTemplateSelector  DefaultDataTemplate   DefaultDataTemplate  ConcisePTCOTemplate   PatternedTCODataTemplate  ConciseSpincoatedTemplate"   SpinCoatedLayerDataTemplate & "ConciseThermallyEvaporatedTemplate+   $ThermallyEvaporatedLayerDataTemplate   ConciseEncapsulationTemplate    EncapsulationDataTemplate��%N   &	        7  7  7  7  7    5   
   8   8  8  8  8  5   -   5      .��5      � 
������� � RowDefinitions 5   
   �� 5      $
��Auto �6   6,   �� 5      $��* �6   6)   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   �� 5      $��2* �6   6*   �� 5      $
��Auto �6   6,   �� 5      $
��Auto �6   6,   .+�5   
   � $��0��6    � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $��* �6#   �� 5       $��* �6#   .+�5"      �� $��0��6   .��6$   !Device templates in this batch:6E   �� 5#      $��1��6   $	��200��6%   $��Remove Selected Template��61   
����d���6q   �� *DeleteSelectedTemplate+6�   �� 5$      $��2��5%      $��Right=�5&      $	��200��5'      $,��&Create Devices and
Go Select Materials��5(      
��� E����5*      �� *AddDevicesFromTemplates+5+      5,      &   System.Windows.Controls.DataGrid  5-   
      AutoGenerateColumns .  	   SelectionMode$	 Single=�50      
 �� SelectedIndex$
 0��51      $ �� IsSynchronizedWithCurrentItem$
 TrueY�52      $��1��53         CanUserAddRows .  ��5-      �� *TemplatesToAdd+ �� UpdateSourceTrigger$ PropertyChanged=� �� SelectedItem 5/      �� *SelectedTemplate+ �� Mode$ TwoWay=�   Columns 55      0  *System.Windows.Controls.DataGridTextColumn  56      ,  &System.Windows.Controls.DataGridColumn   Header$
 Name��6>   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   �� *Name+6N     58      $ # Copies��6H    6%   �� *NumberOfCopies+6\   4  .System.Windows.Controls.DataGridTemplateColumn  59      $ Layers��6)      Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 69      CellTemplate 5:      �� 5;      .R�5<      �� $��0q�6&   #��[  6W   ��61   �� *Device.Layers+��5=   "   �� 5>   &   .R�5?   *   �  K� IsItemsHost . 64   5@   '   5B      5C      5E      5G      � 5H   
   $��3��6    5I      �� 5J      $��* �6#   �� 5K      $��* �6#   �� 5L      $��* �6#   .+�5N      �� $��0��6   .��6$   Available device templates:6A   �� 5O      $��1��5P      $	��200��5Q      $��Center=�5R      $��Add Selected Template��5S      
����d���5U      �� *AddSelectedTemplate+5V      �� 5W      $��2��5X      $	��200��5Y      $��Right=�5Z      $ ��Create New Device Template��5[      
��� E����5]      �� *BuildNewDeviceTemplate+5^      5_        5`   
    .  $	 Single=�5c      $
 TrueY�5d      $��4��5e       .  ��5`      �� *DeviceTemplates+$ TwoWay=�$ PropertyChanged=� 5b      �� *SelectedDeviceTemplate+$ TwoWay=�$ PropertyChanged=� �� FallbackValue$ Nothing�� 5g        5h      $
 Name��6>    6%   �� *Name+6N     5j      $ Layers��6)   $
 Auto 69    5k      �� 5l      .R�5m      �� $��0q�6&   #��[  6W   ��61   �� *Device.Layers+��5n   "   �� 5o   &   .R�5p   *   �  . 64   5q   '   5s      5t      5v      5x      5z      5{      