using UnityEngine;
using System.Collections;

public interface IRemover<T> {

	void Remove(T item);
}
