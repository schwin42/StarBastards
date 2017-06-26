public class ModelPilotInput : PilotInput {
	private float _thrustInput; //TODO Implement output from forward propagation
	public override float ThrustInput { get { return _thrustInput; } }

	private float _turnInput; // TODO Implement output from forward propagation
	public override float TurnInput { get { return _turnInput; } }

	private ShipController shipController; //TODO Will probably need this to get game state
}
