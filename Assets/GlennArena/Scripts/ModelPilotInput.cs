public class ModelPilotInput : IPilotInput {
	private float _thrustInput; //TODO Implement output from forward propagation
	public float ThrustInput { get { return _thrustInput; } }

	private float _turnInput; // TODO Implement output from forward propagation
	public float TurnInput { get { return _turnInput; } }

	private ShipController shipController; //TODO Will probably need this to get game state

	public ModelPilotInput (ShipController shipController) {
		this.shipController = shipController;
	}
}
