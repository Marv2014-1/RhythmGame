import librosa
import json
import os

# Correct the path to your actual file
audio_path = 'Assets/Audio/Music/170BPM.wav'

def extract_beats(audio_path, output_file):
    # Load the audio file
    y, sr = librosa.load(audio_path)
    
    # Extract the tempo and beat frames
    tempo, beat_frames = librosa.beat.beat_track(y=y, sr=sr, hop_length=512)
    
    # Convert the beat frames into time stamps
    beat_times = librosa.frames_to_time(beat_frames, sr=sr)
    
    # Get the directory where this Python file is located
    script_directory = os.path.dirname(os.path.realpath(__file__))
    
    # Form the complete path to save the output file in the same directory as the script
    output_path = os.path.join(script_directory, output_file)
    
    # Save the beat times to a JSON file in the same directory as the script
    with open(output_path, 'w') as f:
        json.dump({'BeatTimes': beat_times.tolist()}, f)
    
    print(f"Output saved to {output_path}")

# Run the beat extraction, specifying the output file name
extract_beats(audio_path, '170BPM.json')
