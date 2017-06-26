using UnityEngine;

public abstract class PilotInput : MonoBehaviour {

	public abstract float ThrustInput { get; }
	public abstract float TurnInput { get; }

	protected ShipController shipController;

	public virtual void Initialize(ShipController shipController) {
		this.shipController = shipController;
	}
}
