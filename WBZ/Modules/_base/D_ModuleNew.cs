using StswExpress;
using StswExpress.Translate;
using System.Windows.Media;
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
        private MV module;
        public MV Module
        {
            get => module;
            set => SetField(ref module, value, () => Module);
        }
        public string Title => TM.Tr(Module.Name + Mode.ToString().Capitalize()) + (InstanceData != null ? $": {(InstanceData as M).Name}" : string.Empty);

        /// Mode
        public Commands.Type Mode { get; set; }
        public ImageSource ModeIcon => Mode switch
        {
            Commands.Type.DUPLICATE => Fn.LoadImage(Properties.Resources.icon32_duplicate),
            Commands.Type.EDIT => Fn.LoadImage(Properties.Resources.icon32_edit),
            Commands.Type.NEW => Fn.LoadImage(Properties.Resources.icon32_add),
            Commands.Type.PREVIEW => Fn.LoadImage(Properties.Resources.icon32_search),
            _ => null
        };

        /// Instance
        private MODULE_MODEL instanceData = new MODULE_MODEL();
        public MODULE_MODEL InstanceData
        {
            get => instanceData;
            set => SetField(ref instanceData, value, () => InstanceData);
        }
    }
}
