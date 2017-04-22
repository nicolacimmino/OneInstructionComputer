using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OneInstructionComputer
{
    static class OISC
    {

        /// <summary>
        /// A lookup table of signal to capture names versus the address they point to.
        /// </summary>
        private static BidirectionalLookup<String, int> signalsToCapture = new BidirectionalLookup<String, int>();

        /// <summary>
        /// ASCII art representation of captured signals, stored by their name.
        /// </summary>
        private static Dictionary<String, String> captureData = new Dictionary<String, String>();

        /// <summary>
        /// Lookup table with location of labels in the code and their name. Used during compilation
        ///     to resolve labels to their address.
        /// </summary>
        private static Dictionary<String, int> labelsLocation = new Dictionary<String, int>();

        /// <summary>
        /// The raw script before it is "compiled".
        /// </summary>
        private static List<String> script = new List<String>();

        /// <summary>
        /// This list contains the script once it has been compiled. It's in a sense the "binary code"
        ///     of the OISC, though it really is still strings. What is here is stripped of comments,
        ///     emprty lines and is fully numeric, ready for exexution.
        /// </summary>
        private static List<String> processedScript = new List<String>();

        /// <summary>
        /// The interrupt flag, if asserted will cause the currently running program to stop.
        /// </summary>
        private static bool interrupt = false;

        /// <summary>
        /// Break signal, if asserted the OISC will stop executing at the next instruction
        ///     and continue when the signal is cleared.
        /// </summary>
        private static bool breakSignal = false;

        /// <summary>
        /// Step forward signal, when asserted, during a break, causes
        ///     the processor to execute one more instruction.
        /// </summary>
        private static bool stepForward = false;

        /// <summary>
        /// The program counter.
        /// </summary>
        private static int pc = 0;

        /// <summary>
        /// Breakpoints. These are original source line numbers.
        ///  It could be good to have this in "compiled line numbers",
        ///  it would only require to look them up once in debugInfo when
        ///  they are assigned.
        /// </summary>
        private static List<int> breakpoints = new List<int>();

        /// <summary>
        /// The thread where the emulation is running.
        /// </summary>
        private static Thread runningThread = null;

        /// <summary>
        /// Debug information keyed by processed code line.
        /// </summary>
        private static Dictionary<int, DebugInfo> debugInfo = new Dictionary<int, DebugInfo>();

        /// <summary>
        /// Current line debug info. Valid only during a break.
        /// </summary>
        private static DebugInfo currentLineDebugInfo = null;

        /// <summary>
        /// Does some cleanup on a user supplied line of code mainly
        ///     replacing tabs and cleaning up duplicated spaces.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static String sanitizeSourceLine(String line)
        {
            // Replace tabs with spaces, trim ends
            String processedLine = line.Replace("\t", " ").Trim();

            // Replace eventual multiple spaces with a single space
            while (processedLine.Contains("  "))
            {
                processedLine = processedLine.Replace("  ", " ");
            }

            return processedLine;
        }

        /// <summary>
        /// Pass one responsibility is to identify all labels, store their position in code
        ///     and cleanup the source from empty and comment lines.
        /// </summary>
        private static void passOne()
        {
            labelsLocation.Clear();
            processedScript.Clear();
            debugInfo.Clear();

            int lineNumber = 0;
            int originalScriptLineNumber = 0;
            foreach (String line in script)
            {
                
                // Skip empty lines, comments and directives.
                if (line.Trim() != "" && !line.Trim().StartsWith(";") && !line.Trim().StartsWith("#"))
                {

                    // Clean up
                    String processedLine = sanitizeSourceLine(line);

                    // Tokenize the line on spaces and see if we have a label, if we do
                    //  store the label location (in code lines).
                    List<String> tokens = new List<string>(processedLine.Split(' '));
                    if (tokens.Count > 0 && tokens[0].EndsWith(":"))
                    {
                        // Store the location of the label along its name
                        labelsLocation.Add(tokens[0].Replace(":", ""), lineNumber);

                        // Remove the label
                        processedLine = processedLine.Replace(tokens[0], "").Trim();
                    }

                    // Store the cleaned line in the processed script so that next 
                    //  passes don't need to do the same clenup.
                    processedScript.Add(processedLine);

                    // Store debug information.
                    DebugInfo thisLineInfo = new DebugInfo();
                    thisLineInfo.OriginalSourceCodeLine = originalScriptLineNumber;
                    debugInfo.Add(lineNumber, thisLineInfo);

                    // One more good line (comments and empty lines are not counted
                    //  cause they don't appear in the processed script).
                    lineNumber++;
                }
                originalScriptLineNumber++;
            }

        }

        /// <summary>
        /// Pass two responsibility is to process the directives (lines starting with #) that allow
        ///     to specify things that are not part of the acutual OISC program like, for instance,
        ///     names of the signals to monitor.
        /// Pass two need preknoweldge gathered in pass one and in particular labels locations.
        /// </summary>
        private static void passTwo()
        {
            signalsToCapture.Clear();

            foreach (String line in script)
            {
                if (line.Trim().StartsWith("#"))
                {
                    // Clean up
                    String directive = sanitizeSourceLine(line);

                    // Tokenize the line
                    List<String> tokens = new List<string>(directive.Split(' '));

                    if (tokens.Count > 0)
                    {
                        if (tokens[0].ToLower() == "#capture")
                        {
                            // Token 1 is the name to be assigned to the capture
                            // Token 2 is the name of the label to attach it to

                            // Ensure we have right amount of tokens
                            if (tokens.Count < 3)
                            {
                                throw new ArgumentException("#capture takes two arguments");
                            }

                            // Ensure label is valid.
                            if (!labelsLocation.ContainsKey(tokens[2]))
                            {
                                throw new ArgumentException("Invalid label " + tokens[2] + " in #capture directive.");
                            }
                            signalsToCapture.Add(tokens[1], labelsLocation[tokens[2]]);
                        }
                    }
                }
            }
        }

        /// Pass three responsibility is to do the final "compilation" that is changing all labels
        ///     to their address. This is because we have only one instruction. In more complex
        ///     intruction sets in this phase we would also do the conversion of the instructions
        ///     to opcodes.
        private static void passThree()
        {

            for (int lineNumber = 0; lineNumber < processedScript.Count; lineNumber++)
            {
                List<String> tokens = new List<string>(processedScript[lineNumber].Split(' '));

                // Token 1 is data
                // Token 2 is copy address
                // Token 3 is jump address

                if (tokens.Count < 3)
                {
                    throw new ArgumentException("Invalid line: " + processedScript[lineNumber]);
                }

                // Replace labels or relative addresses with actual line numbers.
                processedScript[lineNumber] = tokens[0] 
                                                + " " + getLineNumber(tokens[1], lineNumber).ToString() 
                                                + " " + getLineNumber(tokens[2], lineNumber).ToString();
            }

        }

        /// <summary>
        /// Converts a souce token into the relevant address. This includes
        ///     translating labels to addresses but also processing relative
        ///     addresses notations.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static int getLineNumber(String token, int currentLine)
        {
            if (token.StartsWith("+") || token.StartsWith("-"))
            {
                // This is an offset
                return currentLine + int.Parse(token);
            }

            if (token.StartsWith("#"))
            {
                // This is an absolute address
                return int.Parse(token.Replace("#", ""));
            }

            // This is a label
            return labelsLocation[token];
        }

        
        /// <summary>
        /// This is the actual execution loop. In fact these few lines are the only
        ///     actual emulation of the OISC.
        /// </summary>
        private static void doExecute()
        {
            // Reset registers and captures
            pc = 0;
            captureData.Clear();

            while (!interrupt)
            {
                // Fetch debug info for this instruction.
                currentLineDebugInfo = debugInfo[pc];

                if(breakpoints.Contains(currentLineDebugInfo.OriginalSourceCodeLine))
                {
                    // We hit a breakpoint.
                    breakSignal = true;
                    stepForward = false;
                }

                if (!breakSignal)
                {
                    Thread.Sleep(500);
                }
                else
                {
                    if (BreakpointHit != null)
                    {
                        BreakpointHitEventArgs e = new BreakpointHitEventArgs();
                        e.OriginalSourceLine = currentLineDebugInfo.OriginalSourceCodeLine;
                        BreakpointHit(null, e);
                    }
                }

                while (breakSignal)
                {
                    Thread.Sleep(100);
                    if (stepForward)
                    {
                        stepForward = false;
                        break;
                    }
                }

                // Notify we are not anymore in a breakpoint.
                if (BreakpointHit != null)
                {
                    BreakpointHitEventArgs e = new BreakpointHitEventArgs();
                    e.OriginalSourceLine = -1;
                    BreakpointHit(null, e);
                }

                String instruction = processedScript[pc];
                List<String> tokens = new List<String>(instruction.Split(' '));

                // Copy, that is alter the processed script first token
                processedScript[int.Parse(tokens[1])] = tokens[0] + processedScript[int.Parse(tokens[1])].Substring(1);

                // Branch if value was 1 else go to next instruction.
                if (tokens[0] == "1")
                {
                    pc = int.Parse(tokens[2]);
                }
                else
                {
                    pc++;
                }

                doCapture();
            }
        }

        /// <summary>
        /// Sample the value of the data bit on the lines that are to be monitored and
        ///     store the values.
        /// </summary>
        private static void doCapture()
        {

            foreach (int signal in signalsToCapture.ValuesRight())
            {
                String instruction = processedScript[signal];
                List<String> tokens = new List<String>(instruction.Split(' '));

                // This is the first time we capture this signal, add it to the collection
                //  and start the ASCII graphics with the signal name.
                if (!captureData.ContainsKey(signalsToCapture[signal]))
                {
                    captureData.Add(signalsToCapture[signal], signalsToCapture[signal].PadRight(6,' ') + " ");
                }

                if (tokens[0] == "1")
                {
                    captureData[signalsToCapture[signal]] += "-";
                }
                else
                {
                    captureData[signalsToCapture[signal]] += "_";
                }
            }

            if (CaptureDone != null)
            {
                CaptureEventArgs e = new CaptureEventArgs();
                e.CaptureData = captureData;
                CaptureDone(null,e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bpoints"></param>
        public static void SetBreakpoints(List<int> bpoints)
        {
            breakpoints = bpoints;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool Running
        {
            get
            {
                return runningThread != null;
            }
        }

        /// <summary>
        /// Break the execution at the next instruction.
        /// </summary>
        public static void Break()
        {
            breakSignal = true;
        }

        /// <summary>
        /// Step to the next instruction.
        /// </summary>
        public static void Step()
        {
            stepForward = true;
        }

        /// <summary>
        /// Continues to run from a breakpoint.
        /// </summary>
        public static void Continue()
        {
            breakSignal = false;
            stepForward = true;
        }

        /// <summary>
        /// Run the current script.
        /// </summary>
        public static void Run()
        {
            if (runningThread != null)
            {
                throw new InvalidOperationException("A script is already running");
            }

            try
            {
                passOne();
                passTwo();
                passThree();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return;
            }

            runningThread = new Thread(doExecute);
            interrupt = false;
            runningThread.Start();
        }

        /// <summary>
        /// Stop the script execution.
        /// </summary>
        public static void Interrupt()
        {
            interrupt = true;
            runningThread = null;
        }

        /// <summary>
        /// Sets the script to execute.
        /// </summary>
        public static String Script
        {
            set
            {
                script = new List<String>(value.Split('\n'));
            }
        }

        /// <summary>
        /// Capture is done.
        /// </summary>
        public static event CaptureDoneDelegate CaptureDone;

        /// <summary>
        /// Capture is done.
        /// </summary>
        public static event BreakpointHitDelegate BreakpointHit;

        /// <summary>
        /// Event delegate for CaptureDone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CaptureDoneDelegate(object sender, CaptureEventArgs e);

        /// <summary>
        /// Event delegate for BreakpointHit event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void BreakpointHitDelegate(object sender, BreakpointHitEventArgs e);

        /// <summary>
        /// Event args for capture event.
        /// </summary>
        public class CaptureEventArgs : EventArgs
        {
            public Dictionary<String, String> CaptureData;
        }

        /// <summary>
        /// Event args for break point hit event.
        /// </summary>
        public class BreakpointHitEventArgs : EventArgs
        {
            public int OriginalSourceLine;
        }

        /// <summary>
        /// Debug info for a compile code instruction.
        /// </summary>
        private class DebugInfo
        {
            public int OriginalSourceCodeLine;
        }
    }
}
