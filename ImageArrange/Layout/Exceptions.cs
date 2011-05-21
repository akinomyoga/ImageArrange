using Gen=System.Collections.Generic;

namespace afh.Layout{
	[System.Serializable]
	public class LayoutNodeReadOnlyException:System.Exception{
		public LayoutNodeReadOnlyException():base(){}
		public LayoutNodeReadOnlyException(string message):base(message){}
		public LayoutNodeReadOnlyException(string message,System.Exception innerException)
			:base(message,innerException){}
	}
	[System.Serializable]
	public class LayoutNodeFiliationException:System.Exception{
		public LayoutNodeFiliationException():base(){}
		public LayoutNodeFiliationException(string message):base(message){}
		public LayoutNodeFiliationException(string message,System.Exception innerException)
			:base(message,innerException){}
	}
}