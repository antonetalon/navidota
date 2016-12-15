using System.Collections.Generic;

public interface IObserver
{
	void UpdateData(IObservable data);
}

public interface IObservable
{
    void AddObserver(IObserver observer);
    void RemoveObserver(IObserver observer);
}
public abstract class Observable : IObservable
{
	private List<IObserver> _observerList = new List<IObserver>();
	protected virtual void NotifyObservers() {
		foreach (var observer in _observerList)
			observer.UpdateData(this);
	}
	public virtual void AddObserver(IObserver observer) {
		_observerList.Add(observer);
	}
	public virtual void RemoveObserver(IObserver observer) {
		_observerList.Remove(observer);
	}
}
