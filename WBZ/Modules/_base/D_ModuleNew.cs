using StswExpress.Globals;
using WBZ.Models;

namespace WBZ.Modules._base
{
    abstract class D_ModuleNew<MODULE_MODEL> : D where MODULE_MODEL : class, new()
    {
		/// Instance
		private MODULE_MODEL instanceInfo = new MODULE_MODEL();
		public MODULE_MODEL InstanceInfo
		{
			get => instanceInfo;
			set => SetField(ref instanceInfo, value, () => InstanceInfo);
		}

		/// Window mode
		public Commands.Type Mode { get; set; }

		/// Editing mode
		public bool EditingMode => Mode != Commands.Type.PREVIEW;

		/// Additional window icon
		public string ModeIcon
		{
			get
			{
				if		(Mode == Commands.Type.NEW)			return "/Resources/icon32_add.ico";
				else if (Mode == Commands.Type.DUPLICATE)	return "/Resources/icon32_duplicate.ico";
				else if (Mode == Commands.Type.EDIT)		return "/Resources/icon32_edit.ico";
				else										return "/Resources/icon32_search.ico";
			}
		}
	}
}
