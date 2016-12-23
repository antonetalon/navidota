using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchView : MonoBehaviour {

	[SerializeField] Transform _characterView;
	long _characterEntityId = -1;
	bool _isPlaying;
	public void OnStartMatch() {
		gameObject.SetActive(true);
		_isPlaying = true;
		Entity character = null;
		List<Entity> characters = Entities.Find(typeof(PositionComponent), typeof(MovingComponent));
		foreach (var item in characters)
			character = item;
		_characterEntityId = character.Id;
	}
	public void OnEndMatch() {
		gameObject.SetActive(false);
		_isPlaying = false;
	}
	void Update() {
		if (!_isPlaying)
			return;
		UpdateSending();
		UpdatePosition();
	}
	void UpdateSending() {
		if (!Input.GetMouseButtonUp(1))
			return;
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (!Physics.Raycast(ray, out hit))
			return;
		Vector2 targetPos = new Vector2(hit.point.x, hit.point.y);
		InputControlSystem.Instance.Send(targetPos);
	}
	void UpdatePosition() {
		if (_characterEntityId==-1)
			return;
		Entity character = Entities.Find(_characterEntityId);
		Vector2 pos = character.GetComponent<PositionComponent>().Position;
		_characterView.localPosition = new Vector3(pos.x, pos.y, 0);
	}
}
