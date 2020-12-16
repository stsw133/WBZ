﻿using System.Data;
using WBZ.Globals;
using WBZ.Interfaces;
using MODULE_MODEL = WBZ.Models.M_AttributeClass;

namespace WBZ.Modules.AttributesClasses
{
	/// <summary>
	/// Interaction logic for AttributesClassesNew.xaml
	/// </summary>
	public partial class AttributesClassesNew : New
	{
        D_AttributesClassesNew D = new D_AttributesClassesNew();

        public AttributesClassesNew(MODULE_MODEL instance, Commands.Type mode)
        {
            InitializeComponent();
            DataContext = D;
            Init();

            if (instance != null)
                D.InstanceInfo = instance;
            D.Mode = mode;

            D.InstanceInfo.Values = SQL.GetInstancePositions(D.MODULE_TYPE, D.InstanceInfo.ID);
            if (D.Mode == Commands.Type.DUPLICATE)
                foreach (DataRow row in D.InstanceInfo.Values.Rows)
                    row.SetAdded();
        }
    }

    public class New : ModuleNew<MODULE_MODEL> { }
}
