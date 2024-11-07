using Drivetrain = Il2Cpp.CarData.Drivetrain;
using Insomia;

namespace AWDPatches_Model
{
	public interface ITransmissionOptions
	{
        IDrivetrainOptions GetDrivetrain();
        void SetDrivetrain(IDrivetrainOptions value);
    }

	public interface IDrivetrainOptions
	{
        Drivetrain GetType();
        void SetType(Drivetrain value);
    }

	public class DrivetrainOptions : IDrivetrainOptions
	{
        private Drivetrain type;

        public Drivetrain GetType()
        {
            return type;
        }

        public void SetType(Drivetrain value)
        {
            type = value;
        }
    }

	public class BiasedDrivetrainOptions : DrivetrainOptions
	{
        public float BiasedTorque { get; set; } = 0.6f;
	}
}
