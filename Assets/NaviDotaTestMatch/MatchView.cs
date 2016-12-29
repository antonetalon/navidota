using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchView : MonoBehaviour {

	[SerializeField] Transform _characterViewPrefab;
	List<Transform> _characterViews;

	List<long> _characterEntityIds;
	bool _isPlaying;
	public void OnStartMatch() {
		_characterViewPrefab.gameObject.SetActive (false);
		_characterEntityIds = new List<long> ();
		_characterViews = new List<Transform> ();
		gameObject.SetActive(true);
		_isPlaying = true;
		List<Entity> characters = GetCharacterEntities ();
		foreach (var item in characters)
			AddEntity (item.Id);
	}
	private void AddEntity(long entityId) {
		Transform view = Instantiate (_characterViewPrefab) as Transform;
		view.gameObject.SetActive (true);
		_characterViews.Add (view);
		_characterEntityIds.Add(entityId);
	}
	private List<Entity> GetCharacterEntities() {
		return Entities.Find(typeof(PositionComponent), typeof(MovingComponent));;
	}
	public void OnEndMatch() {
		gameObject.SetActive(false);
		_isPlaying = false;
		for (int i = _characterEntityIds.Count-1; i >=0; i--) {
			_characterEntityIds.RemoveAt (i);
			Destroy(_characterViews[i].gameObject);
			_characterViews.RemoveAt (i);
		}
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
		var currPos = Entities.Find (_characterEntityIds [0]).GetComponent<PositionComponent> ().Position;
		Debug.LogFormat("sent from [{0};{1}] to [{2};{3}] at {4}",  currPos.x, currPos.y, targetPos.x, targetPos.y, Timer.Time);
	}
	void UpdatePosition() {
		for (int i = _characterEntityIds.Count-1; i >=0; i--) {
			Entity character = Entities.Find (_characterEntityIds[i]);
			bool exists = false;
			if (character != null) {
				var pos = character.GetComponent<PositionComponent> ();
				if (pos != null) {
					_characterViews [i].localPosition = new Vector3 (pos.Position.x, pos.Position.y, 0);
					exists = true;
				}
			}
			// Remove deleted.
			if (!exists) {
				_characterEntityIds.RemoveAt (i);
				Destroy(_characterViews[i].gameObject);
				_characterViews.RemoveAt (i);
			}
		}
		// Add new.
		List<Entity> entities = GetCharacterEntities();
		foreach (var entity in entities) {
			if (_characterEntityIds.IndexOf (entity.Id) != -1)
				continue;
			AddEntity (entity.Id);
		}
	}
}
