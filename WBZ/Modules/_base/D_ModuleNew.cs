using StswExpress;
using StswExpress.Translate;
using WBZ.Globals;
using WBZ.Models;

namespace WBZ.Modules._base
{
    abstract class D_ModuleNew<MODULE_MODEL> : D where MODULE_MODEL : class, new()
    {
		public D_ModuleNew()
		{
			Module = Config.GetModule(GetType().Name[2..^3]);
		}

		/// Module
		public MV Module;
		public string Title => TM.Tr(Module.Name + Mode.ToString().Capitalize()) + (InstanceData != null ? $": {(InstanceData as M).Name}" : string.Empty);

		/// Mode
		public Commands.Type Mode { get; set; }
		public string ModeIcon
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return "/Resources/32/icon32_add.ico";
				else if (Mode == Commands.Type.DUPLICATE)	return "/Resources/32/icon32_duplicate.ico";
				else if (Mode == Commands.Type.EDIT)		return "/Resources/32/icon32_edit.ico";
				else if (Mode == Commands.Type.PREVIEW)		return "/Resources/32/icon32_search.ico";
				else										return null;
			}
		}

		/// Instance
		private MODULE_MODEL instanceData = new MODULE_MODEL();
		public MODULE_MODEL InstanceData
		{
			get => instanceData;
			set => SetField(ref instanceData, value, () => InstanceData);
		}
	}
}
