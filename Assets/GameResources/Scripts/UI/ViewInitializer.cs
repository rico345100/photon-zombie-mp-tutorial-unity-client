using UnityEngine;

public class ViewInitializer : MonoBehaviour {
	public ViewBase entryView;

	void Awake() {
		ViewBase[] views = GameObject.FindObjectsOfType<ViewBase>();

		for(int i = 0; i < views.Length; i++) {
			ViewBase view = views[i];

			// Force to activate objects to initiate ViewBase Component
			view.viewObject.SetActive(true);

			if(view == entryView) {
				view.Show();
				continue;
			}
			
			// Force to deactivate all views
			view.viewObject.SetActive(false);
		}
	}
}
