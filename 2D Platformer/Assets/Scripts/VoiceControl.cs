using UnityEngine;
using System.Collections;

public class VoiceControl : MonoBehaviour {
	

	// Sampling rate of microphone.
	public const int samplingRate = 44100;
	// Audio source. It will be connected with microphone.
	AudioSource audio;

	// Number of frames currently used for calibration.
	int frameCountCalibrating;
	// Number of frames currently used for prediction.
	int frameCountPredict;
	// Array used to buffered spectrum data of player's samples and prediction.
	private float[] spectrum_jump;
	private float[] spectrum_shoot;
	private float[] spectrum_predict;

	// Send commands to game engine. (See PlayerControl.cs and Gun.cs)
	public static bool jump;
	public static bool shoot;


	void Start() {
		
		// Initialize audio source and microphone.
		audio = GetComponent<AudioSource>();
		audio.clip = Microphone.Start(null, true, 1, samplingRate);
		audio.loop = true;
		while (!(Microphone.GetPosition(null) > 0)){}
		audio.Play();

		// Initilize arrays and reset variables.
		jump = false;
		shoot = false;
		frameCountCalibrating = 0;
		spectrum_jump = new float[QSamples];
		spectrum_shoot = new float[QSamples];
		spectrum_predict = new float[QSamples];
		_spectrum = new float[QSamples];

	}


	void Update () {

		// Prepare spectrum data of current frame.
		getSpectrum();

		// Player press and hold "J" button to calibrate "Jump" action.
		if (Input.GetKey (KeyCode.J)) {
			// Collect data for calibration.
			if(PitchValue != 0) {
				// Add current frame spectrum to "Jump" spectrum.
				for (int i = 0; i < spectrum_jump.Length; i++) {
					spectrum_jump[i] += _spectrum[i];
				}
				frameCountCalibrating++;
			}
			// End of calibration.
			else if (PitchValue == 0 && frameCountCalibrating > 0) {
				// Calculate the average spectrum for "Jump" action. This will be used as a training spectrum for prediction.
				for (int i = 0; i < spectrum_jump.Length; i++) {
					spectrum_jump [i] /= frameCountCalibrating;
				}
				frameCountCalibrating = 0;
				Debug.Log ("Calibrate \"Jump\" successfully.");
			}
		}

		// Player press and hold "S" button to calibrate "Shoot" action.
		else if (Input.GetKey (KeyCode.S)) {
			// Collect data for calibration.
			if(PitchValue != 0) {
				// Add current frame spectrum to "Shoot" spectrum.
				for (int i = 0; i < spectrum_shoot.Length; i++) {
					spectrum_shoot[i] += _spectrum[i];
				}
				frameCountCalibrating++;
			}
			// End of calibration.
			else if (PitchValue == 0 && frameCountCalibrating > 0) {
				// Calculate the average spectrum for "Shoot" action. This will be used as a training spectrum for prediction.
				for (int i = 0; i < spectrum_shoot.Length; i++) {
					spectrum_shoot [i] /= frameCountCalibrating;
				}
				frameCountCalibrating = 0;
				Debug.Log ("Calibrate \"Shoot\" successfully.");
			} 
		}

		// Predict action.
		else {
			// Collect data for prediction.
			if(PitchValue != 0) {
				for (int i = 0; i < spectrum_predict.Length; i++) {
					spectrum_predict[i] += _spectrum[i];
				}
				frameCountPredict++;
			}
			// Predict action.
			else if (PitchValue == 0 && frameCountPredict > 0) {
				// Calculate the average spectrum for prediction.
				for (int i = 0; i < spectrum_predict.Length; i++) {
					spectrum_predict [i] /= frameCountPredict;
				}
				frameCountPredict = 0;
				// Predict action.
				int action = predictAction();
				// Predict "Jump"
				if (action == 0) {
					Debug.Log ("Predict: Jump");
					jump = true;
				}
				// Predict "Shoot"
				else {
					Debug.Log ("Predict: Shoot");
					shoot = true;
				}
			}
		}

	}


	// Predict action using similarity.
	int predictAction() {
		// Calculate similarity of current sprectrum and a training spectrum of "Jump" action.
		double diff_jump = 0;
		for (int i = 0; i < spectrum_jump.Length; i++) {
			diff_jump += Mathf.Abs(spectrum_jump [i] - spectrum_predict [i]) * Mathf.Abs(spectrum_jump [i] - spectrum_predict [i]);
		}
		// Calculate similarity of current sprectrum and a training spectrum of "Shoot" action.
		double diff_shoot = 0;
		for (int i = 0; i < spectrum_jump.Length; i++) {
			diff_shoot += Mathf.Abs(spectrum_shoot [i] - spectrum_predict [i]) * Mathf.Abs(spectrum_shoot [i] - spectrum_predict [i]);
		}
		Debug.Log("Diff: " + diff_jump + " " + diff_shoot);
		// Predict "Jump"
		if (diff_jump < diff_shoot) {
			return 0;
		}
		// Predict "Shoot"
		else {
			return 1;
		}
	}
		

	public float PitchValue;
	private const int QSamples = 2048;
	private const float Threshold = 0.02f;
	private float[] _spectrum;
	// Calculate spectrum of current frame of voice.
	void getSpectrum()
	{
		audio.GetSpectrumData(_spectrum, 1, FFTWindow.BlackmanHarris);
		// Ref: http://answers.unity3d.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
		float maxV = 0;
		var maxN = 0;
		for (int i = 0; i < QSamples; i++)
		{ // find max 
			if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
				continue;
			maxV = _spectrum[i];
			maxN = i; // maxN is the index of max
		}
		float freqN = maxN; // pass the index to a float variable
		if (maxN > 0 && maxN < QSamples - 1)
		{ // interpolate index using neighbours
			var dL = _spectrum[maxN - 1] / _spectrum[maxN];
			var dR = _spectrum[maxN + 1] / _spectrum[maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}
		PitchValue = freqN * (samplingRate / 2) / QSamples; // convert index to frequency
	}

}