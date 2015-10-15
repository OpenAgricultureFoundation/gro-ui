using UnityEngine;
using System.Collections;

public interface ISelectionReceiver<T>
{
	void MakeSelection(T selection);

}
