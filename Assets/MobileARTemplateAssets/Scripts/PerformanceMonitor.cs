using System.Text;
using Unity.Profiling;
using UnityEngine;
using TMPro; 

public class PerformanceMonitor : MonoBehaviour
{
    public TextMeshProUGUI statsText; // Assign your UI text here
    
    // Recorders for different metrics
    ProfilerRecorder totalUsedMemoryRecorder;  // actual amount of RAM currently allocated by the application
    ProfilerRecorder gcReservedMemoryRecorder; // memory reserved by the GC, which may be larger than the actual used memory
    ProfilerRecorder gpuFrameTimeRecorder;     // time taken by the GPU to render the current frame
    ProfilerRecorder drawCallsRecorder;        // number of draw calls issued in the current frame

    void OnEnable()
    {
        // Initialize the recorders
        totalUsedMemoryRecorder  = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
        gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        gpuFrameTimeRecorder     = ProfilerRecorder.StartNew(ProfilerCategory.Render, "GPU Frame Time");
        drawCallsRecorder        = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
    }

    void OnDisable()
    {
        totalUsedMemoryRecorder.Dispose();
        gcReservedMemoryRecorder.Dispose();
        gpuFrameTimeRecorder.Dispose();
        drawCallsRecorder.Dispose();
    }

    void Update()
    {
        var sb = new StringBuilder(200); // Pre-allocate string builder capacity for better performance

        // Convert bytes to Megabytes for readability
        double usedMem = totalUsedMemoryRecorder.LastValue / (1024 * 1024);
        double gcMem = gcReservedMemoryRecorder.LastValue / (1024 * 1024);
        
        // Convert GPU nanoseconds to milliseconds
        double gpuMs = gpuFrameTimeRecorder.LastValue * (1e-6f);

        sb.AppendLine($"Total Mem : {usedMem:F1} MB"); // actual amount of RAM currently allocated by the application
        sb.AppendLine($"GC Mem    : {gcMem:F1} MB");   // memory reserved by the GC, which may be larger than the actual used memory
        sb.AppendLine($"GPU Time  : {gpuMs:F2} ms");   // time taken by the GPU to render the current frame
        // sb.AppendLine($"Draw Calls: {drawCallsRecorder.LastValue}"); // number of draw calls issued in the current frame

        statsText.text = sb.ToString();
    }
}