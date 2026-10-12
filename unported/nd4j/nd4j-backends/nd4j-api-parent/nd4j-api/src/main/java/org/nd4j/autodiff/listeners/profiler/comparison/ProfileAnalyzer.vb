Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports ProfilingListener = org.nd4j.autodiff.listeners.profiler.ProfilingListener
Imports Phase = org.nd4j.autodiff.listeners.profiler.data.Phase
Imports TraceEvent = org.nd4j.autodiff.listeners.profiler.data.TraceEvent
Imports TraceEvents = org.nd4j.autodiff.listeners.profiler.data.TraceEvents
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives
Imports NDArrayList = org.nd4j.list.NDArrayList
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 
Namespace org.nd4j.autodiff.listeners.profiler.comparison


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ProfileAnalyzer
	Public Class ProfileAnalyzer

		''' <summary>
		''' Chrome profiler supports 2 formats:<br>
		''' SameDiff == JSON Array Format<br>
		''' TensorFlow == JSON Object Format<br>
		''' </summary>
		Public Enum ProfileFormat
			SAMEDIFF
			TENSORFLOW

		''' <summary>
		''' Only applicable for profile comparisons.<br>
		''' PROFILE1_PC - sort by profile 1 percentage of total time<br>
		''' PROFILE2_PC - sort by profile 2 percentage of total time<br>
		''' RATIO - sort by highest ratio (mean op time profile 1 / mean op time profile 2)
		''' </summary>
		End Enum
		Public Enum SortBy
			PROFILE1_PC
			PROFILE2_PC
			RATIO

		''' <summary>
		''' TEXT: Human readable, columns padded for alignment<br>
		''' CSV: CSV format, comma separated
		''' </summary>
		End Enum
		Public Enum OutputFormat
			TEXT
			CSV


		''' <summary>
		''' Summarize and print to stdout the specified profile file
		''' </summary>
		''' <param name="file">          Profile file </param>
		''' <param name="profileFormat"> Format of the profiler file </param>
		End Enum
		Public Shared Sub summarizeProfile(ByVal file As File, ByVal profileFormat As ProfileFormat)
			Console.WriteLine(summarizeProfileStr(file, profileFormat))
		End Sub

		''' <summary>
		''' Summarize and return as a string the specified profile file
		''' </summary>
		''' <param name="file">          Profile file </param>
		''' <param name="profileFormat"> Format of the profiler file </param>
		Public Shared Function summarizeProfileStr(ByVal file As File, ByVal profileFormat As ProfileFormat) As String
			Dim events() As TraceEvent = getTraceEvents(file, profileFormat)
			Return summarizeTraceEvents(events)
		End Function

		''' <summary>
		''' Aggregate, summarize and print to stdout all .json profile files in the specified directory (not recursive)
		''' </summary>
		''' <param name="dir">           Directory containing the profiles </param>
		''' <param name="profileFormat"> Profile format </param>
		Public Shared Sub summarizeProfileDirectory(ByVal dir As File, ByVal profileFormat As ProfileFormat)
			Console.WriteLine(summarizeProfileDirectoryStr(dir, profileFormat))
		End Sub

		''' <summary>
		''' Aggregate, summarize and return as a String all .json profile files in the specified directory (not recursive)
		''' </summary>
		''' <param name="dir">           Directory containing the profiles </param>
		''' <param name="profileFormat"> Profile format </param>
		Public Shared Function summarizeProfileDirectoryStr(ByVal dir As File, ByVal profileFormat As ProfileFormat) As String
			Return summarizeTraceEvents(getTraceEventsDir(dir, profileFormat))
		End Function

		''' <summary>
		''' Load, aggregate and return the TraceEvent object from all profiles in the specified directory
		''' </summary>
		''' <param name="dir">           Directory containing the profiles </param>
		''' <param name="profileFormat"> Profile format </param>
		Public Shared Function getTraceEventsDir(ByVal dir As File, ByVal profileFormat As ProfileFormat) As TraceEvent()
			Dim files() As File = dir.listFiles()
			Preconditions.checkState(files IsNot Nothing AndAlso files.Length > 0, "No profiles found in directory: %s", dir)
			Dim l As IList(Of TraceEvent) = New List(Of TraceEvent)()
			For Each f As File In files
				If Not f.getName().EndsWith(".json") Then
					log.info("Skipping non-JSON file in directory - {}", f.getAbsolutePath())
					Continue For
				End If
				Dim e() As TraceEvent = getTraceEvents(f, profileFormat)
				Collections.addAll(l, e)
			Next f
			Return CType(l, List(Of TraceEvent)).ToArray()
		End Function

		''' <summary>
		''' Load and return the TraceEvent object from the specified profile file
		''' </summary>
		''' <param name="file">          Profile file </param>
		''' <param name="profileFormat"> Profile format </param>
		Public Shared Function getTraceEvents(ByVal file As File, ByVal profileFormat As ProfileFormat) As TraceEvent()
			Return getTraceEvents(file, profileFormat, True)
		End Function

		Public Shared Function getTraceEvents(ByVal file As File, ByVal profileFormat As ProfileFormat, ByVal aggregateTFSubOps As Boolean) As TraceEvent()
			Dim json As ObjectMapper = ProfilingListener.jsonMapper()

			Dim content As String = Nothing
			Try
					Using bufferedInputStream As New BufferedInputStream(New FileStream(file, FileMode.Open, FileAccess.Read))
					Try
						content = IOUtils.toString(bufferedInputStream, Charset.defaultCharset())
					Catch e As IOException
						Throw New Exception(e)
					End Try
					End Using
			Catch e As FileNotFoundException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			Catch e As IOException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try


			If Not content.matches(".*]\s*") Then
				If content.EndsWith(",", StringComparison.Ordinal) Then
					'Has comma, missing ]
					content = content.Substring(0, content.Length - 1) & "]"
				ElseIf content.EndsWith("," & vbLf, StringComparison.Ordinal) Then
					'Has comma and newline, missing ]
					content = content.Substring(0, content.Length - 2) & "]"
				Else
					content = content & "]"
				End If
			End If

			Dim events() As TraceEvent
			If profileFormat = ProfileFormat.SAMEDIFF Then
				Try
					events = json.readValue(content, GetType(TraceEvent()))
				Catch e As IOException
					Throw New Exception(e)
				End Try
			Else
				'TF format
				Dim traceEvents As TraceEvents
				Try
					traceEvents = json.readValue(content, GetType(TraceEvents))
				Catch e As IOException
					Throw New Exception(e)
				End Try
				events = traceEvents.getTraceEvents().toArray(New TraceEvent(){})

				'Clean up TF format - sometimes things like "Softmax" are actually profiled as "_MklSoftmax"
				'And we'll align TF names to SameDiff names
				For Each te As TraceEvent In events
					If TF_PROFILE_ALIASES.ContainsKey(te.getName()) Then
						te.setName(TF_PROFILE_ALIASES(te.getName()))
					End If

					Dim df As DifferentialFunction = DifferentialFunctionClassHolder.Instance.getOpWithTensorflowName(te.getName())
					If df IsNot Nothing Then
						te.setName(df.opName())
					End If
				Next te


				If aggregateTFSubOps Then
					'For CUDA ops, TF will log sub-ops like:
					'fire2/e1x1/Conv2D:Conv2D#id=74,device=/job:localhost/replica:0/task:0/device:GPU:0,async=false#@@cudnn::maxwell::gemm::computeOffsetsKernel(cudnn::maxwell::gemm::ComputeOffsetsParams)
					'fire2/e1x1/Conv2D:Conv2D#id=74,device=/job:localhost/replica:0/task:0/device:GPU:0,async=false#@@maxwell_scudnn_128x64_relu_interior_nn
					'fire2/e1x1/Conv2D:Conv2D#id=74,device=/job:localhost/replica:0/task:0/device:GPU:0,async=false#@@void tensorflow::functor::ShuffleInTensor3Simple<float, 2, 1, 0, false>(int, float const*, tensorflow::functor::Dimension<3>, float*)
					'We'll join these into one op, then strip everything after the ":" to recover the op name

					'Also, TF has multiple sub-ops like this, sequentially, that need to be joined:
					'19 = {TraceEvent@3157} "TraceEvent(name=Conv2D#id=80,device=/job, categories=null, ph=X, ts=1576896601259742, dur=466, tts=null, pid=5, tid=0, args={name=conv1/Conv2D, op=Conv2D#id=80,device=/job}, cname=null)"
					'20 = {TraceEvent@3181} "TraceEvent(name=Conv2D#id=80,device=/job, categories=null, ph=X, ts=1576896601260229, dur=29, tts=null, pid=5, tid=0, args={name=conv1/Conv2D, op=Conv2D#id=80,device=/job}, cname=null)"
					'21 = {TraceEvent@3206} "TraceEvent(name=Conv2D#id=80,device=/job, categories=null, ph=X, ts=1576896601260329, dur=31, tts=null, pid=5, tid=0, args={name=conv1/Conv2D, op=Conv2D#id=80,device=/job}, cname=null)"
					'22 = {TraceEvent@3247} "TraceEvent(name=Conv2D#id=80,device=/job, categories=null, ph=X, ts=1576896601260390, dur=4998, tts=null, pid=5, tid=0, args={name=conv1/Conv2D, op=Conv2D#id=80,device=/job}, cname=null)"

					Dim map As IDictionary(Of String, TraceEvent) = New Dictionary(Of String, TraceEvent)() 'Key: Op name with ID
					Dim [out] As IList(Of TraceEvent) = New List(Of TraceEvent)()
					Dim last As TraceEvent = Nothing
					For Each te As TraceEvent In events
						If last IsNot Nothing AndAlso last.getPh() = Phase.X AndAlso te.getPh() = Phase.X AndAlso last.getName().Equals(te.getName()) AndAlso last.getArgs() IsNot Nothing AndAlso te.getArgs() IsNot Nothing AndAlso last.getArgs().get("name").Equals(te.getArgs().get("name")) AndAlso last.getArgs().get("op").Equals(te.getArgs().get("op")) Then
							'Aggregate - same names, ops, etc
							last.setDur(last.getDur() + te.getDur())
							Continue For
						End If

						last = te
						If te.getArgs() Is Nothing OrElse te.getArgs().isEmpty() Then
							[out].Add(te)
							Continue For
						End If


						Dim n As String = CStr(te.getArgs().get("name"))

						'Aggregate by op name...
						'"fire2/e1x1/Conv2D:Conv2D#id=74,device=/job:localhost/replica:0/..." -> "fire2/e1x1/Conv2D"
						'We're relying on TF's "one iteration per json file" here
						If n.matches("[\w/_-]+:[\w/_-]+#id=\d+.*") Then
							Dim idx As Integer = n.IndexOf("#", StringComparison.Ordinal)
							Dim sub1 As String = n.Substring(0, idx)
							Dim [sub] As String
							If sub1.Contains(":") Then
								[sub] = sub1.Substring(0, sub1.LastIndexOf(":", StringComparison.Ordinal))
							Else
								[sub] = sub1
							End If
							If map.ContainsKey([sub]) Then
								Dim t As TraceEvent = map([sub])
								Dim dur As Long? = t.getDur()
								If dur Is Nothing AndAlso te.getDur() Is Nothing Then
									Continue For
								End If
								t.setDur(If(dur Is Nothing, te.getDur(), dur.Value + (If(te.getDur() Is Nothing, 0, te.getDur()))))
							Else
								map([sub]) = te
								[out].Add(te)
							End If
						Else
							If map.ContainsKey(n) Then
								Dim t As TraceEvent = map(n)
								t.setDur(t.getDur() + te.getDur())
							Else
								map(n) = te
								[out].Add(te)
							End If
						End If
					Next te

					'Strip everything after ":" in "fire2/e1x1/Conv2D:Conv2D#id=74,device=/job:localhost/..."
					For i As Integer = 0 To [out].Count - 1
						Dim te As TraceEvent = [out](i)
						If te.getArgs() Is Nothing OrElse te.getArgs().isEmpty() Then
							Continue For
						End If

						Dim n As String = CStr(te.getArgs().get("name"))
						If n.matches("[\w/_-]+:[\w/_-]+#id=\d+.*") Then
							Dim idx As Integer = n.IndexOf(":"c)
							Dim [sub] As String = n.Substring(0, idx)
							te.getArgs().put("name", [sub])
						End If
					Next i

					events = CType([out], List(Of TraceEvent)).ToArray()
				End If
			End If

			Return events
		End Function

		''' <summary>
		''' Summarize the specified TraceEvents as a String
		''' </summary>
		''' <param name="events"> Events to summarize </param>
		Public Shared Function summarizeTraceEvents(ByVal events() As TraceEvent) As String
			Dim p As Pair(Of Long, IDictionary(Of String, OpStats)) = aggregateTraceEvents(events)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Map<String, OpStats> stats = p.getSecond();
			Dim stats As IDictionary(Of String, OpStats) = p.Second
			Dim allOpsUs As Long = p.First

			'Summarize by op type:
			Dim l As IList(Of String) = New List(Of String)(stats.Keys)
			l.Sort(New ComparatorAnonymousInnerClass(stats))

			'Work out longest name and op name:
			Dim longestName As Integer = 30
			Dim longestOpName As Integer = 30
			For Each s As String In l
				longestName = Math.Max(longestName, s.Length + 1)
				longestOpName = Math.Max(longestOpName, stats(s).getOpName().length() + 1)
			Next s

			Dim sb As New StringBuilder()
			Dim headerFormat As String = "%-" & longestName & "s%-" & longestOpName & "s%-10s%-10s%-10s%-10s%-10s%-10s" & vbLf
			sb.Append(String.format(headerFormat, "Op Name", "Op", "Count", "Total uS", "%", "Min", "Max", "Std"))
			Dim format As String = "%-" & longestName & "s%-" & longestOpName & "s%-10d%-10d%-10.2f%-10d%-10d%-10.2f" & vbLf
			For Each s As String In l
				Dim st As OpStats = stats(s)
				Dim pc As Double = (100.0 * st.getSumUs()) / allOpsUs
				Dim arr As INDArray = st.getTimesUs().array()
				Dim min As Long = arr.minNumber().longValue()
				Dim max As Long = arr.maxNumber().longValue()
				Dim std As Double = arr.stdNumber().doubleValue()
				sb.Append(String.format(format, s, st.getOpName(), st.getCount(), st.getSumUs(), pc, min, max, std))
			Next s

			Return sb.ToString()
		End Function

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of String)

			Private stats As IDictionary(Of String, OpStats)

			Public Sub New(ByVal stats As IDictionary(Of String, OpStats))
				Me.stats = stats
			End Sub

			Public Function Compare(ByVal o1 As String, ByVal o2 As String) As Integer Implements IComparer(Of String).Compare
				Return -Long.compare(stats(o1).getSumUs(), stats(o2).getSumUs())
			End Function
		End Class

		Private Shared Function aggregateTraceEvents(ByVal events() As TraceEvent) As Pair(Of Long, IDictionary(Of String, OpStats))
			'Summarize by op (instance) name:
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Map<String, OpStats> stats = new HashMap<>();
			Dim stats As IDictionary(Of String, OpStats) = New Dictionary(Of String, OpStats)()
			For Each e As TraceEvent In events
				If e.getPh() <> Phase.X OrElse e.getDur() Is Nothing Then
					Continue For
				End If

				Dim s As OpStats
				Dim instanceName As String = CStr(e.getArgs().get("name"))
				If stats.ContainsKey(instanceName) Then
					s = stats(instanceName)
				Else
					s = New OpStats(instanceName, e.getName(), 0, New NDArrayList(DataType.LONG, 0), Nothing)
					stats(instanceName) = s
				End If
				s.setCount(s.getCount() + 1)
				s.getTimesUs().add(CDbl(e.getDur()))
			Next e

			Dim allOpsUs As Long = 0
			For Each s As OpStats In stats.Values
				s.setSumUs(s.getTimesUs().array().sumNumber().longValue())
				allOpsUs += s.getSumUs()
			Next s

			Return New Pair(Of Long, IDictionary(Of String, OpStats))(allOpsUs, stats)
		End Function
		''' <summary>
		''' Compare the specified profile files, sorted by profile 1 % of total time
		''' </summary>
		''' <param name="file1">   First profile file </param>
		''' <param name="file2">   Second profile file </param>
		''' <param name="format1"> Format of first profile </param>
		''' <param name="format2"> Format of second profile </param>
		''' <returns> Comparison summary as a String </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static String compareProfiles(@NonNull File file1, @NonNull File file2, @NonNull ProfileFormat format1, @NonNull ProfileFormat format2)
		Public Shared Function compareProfiles(ByVal file1 As File, ByVal file2 As File, ByVal format1 As ProfileFormat, ByVal format2 As ProfileFormat) As String
			Return compareProfiles(file1, file2, format1, format2, False, False, Nothing, Nothing, SortBy.PROFILE1_PC)
		End Function

		''' <summary>
		''' Compare the specified profile files or directory
		''' </summary>
		''' <param name="file1">       First profile file or directory of profiles </param>
		''' <param name="file2">       Second profile file or directory of profiles </param>
		''' <param name="format1">     Format for first profile file/s </param>
		''' <param name="format2">     Format for second profile file/s </param>
		''' <param name="firstIsDir">  True if the first File object is a directory </param>
		''' <param name="secondIsDir"> True if the second File object is a directory </param>
		''' <param name="name1">       Name of the first profile (just for display purposes). Optional </param>
		''' <param name="name2">       Name of the second profile (just for display purposes). Optional </param>
		''' <param name="sortBy">      What to sort the summary results by </param>
		''' <returns> Comparison summary as a String </returns>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static String compareProfiles(@NonNull File file1, @NonNull File file2, @NonNull ProfileFormat format1, @NonNull ProfileFormat format2, boolean firstIsDir, boolean secondIsDir, String name1, String name2, final SortBy sortBy)
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
		Public Shared Function compareProfiles(ByVal file1 As File, ByVal file2 As File, ByVal format1 As ProfileFormat, ByVal format2 As ProfileFormat, ByVal firstIsDir As Boolean, ByVal secondIsDir As Boolean, ByVal name1 As String, ByVal name2 As String, ByVal sortBy As SortBy) As String
			Return compareProfiles(Config.builder().profile1(file1).profile2(file2).profile1Format(format1).profile2Format(format2).profile1IsDir(firstIsDir).profile2IsDir(secondIsDir).p1Name(name1).p2Name(name2).sortBy(sortBy).build())
		End Function

'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: public static String compareProfiles(final Config c)
		Public Shared Function compareProfiles(ByVal c As Config) As String
			Dim t1() As TraceEvent = If(c.profile1IsDir(), getTraceEventsDir(c.profile1(), c.profile1Format()), getTraceEvents(c.profile1(), c.profile1Format()))
			Dim t2() As TraceEvent = If(c.profile2IsDir(), getTraceEventsDir(c.profile2(), c.profile2Format()), getTraceEvents(c.profile2(), c.profile2Format()))

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.common.primitives.Pair<Long, Map<String, OpStats>> p1 = aggregateTraceEvents(t1);
			Dim p1 As Pair(Of Long, IDictionary(Of String, OpStats)) = aggregateTraceEvents(t1)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.common.primitives.Pair<Long, Map<String, OpStats>> p2 = aggregateTraceEvents(t2);
			Dim p2 As Pair(Of Long, IDictionary(Of String, OpStats)) = aggregateTraceEvents(t2)

			Dim l As IList(Of String) = New List(Of String)(If(c.sortBy() <> SortBy.PROFILE2_PC, p1.Second.Keys, p2.Second.Keys))
			l.Sort(New ComparatorAnonymousInnerClass2(c, p1, p2))

			Dim set As ISet(Of String) = New HashSet(Of String)(l)


			Dim sb As New StringBuilder()
			sb.Append("1 = ").Append(If(c.p1Name() Is Nothing, "Profile 1", c.p1Name())).Append(vbLf).Append("2 = ").Append(If(c.p2Name() Is Nothing, "Profile 2", c.p2Name())).Append(vbLf)

			'Work out longest name and op name:
			Dim longestName As Integer = 30
			Dim longestOpName As Integer = 30
			Dim stats As IDictionary(Of String, OpStats) = If(c.sortBy() = SortBy.PROFILE2_PC, p2.Second, p1.Second)
			For Each s As String In l
				longestName = Math.Max(longestName, s.Length + 1)
				longestOpName = Math.Max(longestOpName, stats(s).getOpName().length() + 1)
			Next s

			Dim headerFormat As String
			Dim format As String
			If c.format() Is Nothing OrElse c.format() = OutputFormat.TEXT Then
				headerFormat = "%-" & longestName & "s%-" & longestOpName & "s%-10s%-10s%-16s%-13s%-13s%-14s%-14s%-12s%-12s%-14s%-14s%-10s%-10s%-10s%-10s" & vbLf
				format = "%-" & longestName & "s%-" & longestOpName & "s%-10d%-10d%-16.2f%-13.2f%-13.2f%-14d%-14d%-12.2f%-12.2f%-14d%-14d%-10d%-10d%-10.2f%-10.2f" & vbLf
			Else
				headerFormat = "%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s" & vbLf
				format = "%s,%s,%d,%d,%.2f,%.2f,%.2f,%d,%d,%.2f,%.2f,%d,%d,%d,%d,%.2f,%.2f" & vbLf
			End If
			sb.Append(String.format(headerFormat, "Op Name", "Op", "Count (1)", "Count (2)", "Mean Ratio 1/2", "Mean (1)", "Mean (2)", "Total uS (1)", "Total uS (2)", "% (1)", "% (2)", "Min (1)", "Min (2)", "Max (1)", "Max (2)", "Std (1)", "Std (2)"))


			For Each s As String In l
				Dim s1 As OpStats = p1.Second(s)
				Dim s2 As OpStats = p2.Second(s)

				If c.filter() IsNot Nothing AndAlso Not c.filter().apply(s1, s2) Then
					Continue For
				End If

				Dim m1 As Double = If(s1 Is Nothing, 0, s1.getTimesUs().array().meanNumber().doubleValue())
				Dim m2 As Double = If(s2 Is Nothing, 0, s2.getTimesUs().array().meanNumber().doubleValue())
				Dim ratio As Double = m1 / m2

				Dim pc1 As Double = If(s1 Is Nothing, 0, 100.0 * s1.getSumUs() / p1.First)
				Dim pc2 As Double = If(s2 Is Nothing, 0, 100.0 * s2.getSumUs() / p2.First)

				sb.Append(String.format(format, s,If(s1 IsNot Nothing, s1.getOpName(), s2.getOpName()),If(s1 IsNot Nothing, s1.getCount(), 0),If(s2 IsNot Nothing, s2.getCount(), 0), ratio, m1, m2,If(s1 IsNot Nothing, s1.getSumUs(), 0),If(s2 IsNot Nothing, s2.getSumUs(), 0), pc1, pc2,If(s1 IsNot Nothing, s1.getTimesUs().array().minNumber().longValue(), 0),If(s2 IsNot Nothing, s2.getTimesUs().array().minNumber().longValue(), 0),If(s1 IsNot Nothing, s1.getTimesUs().array().maxNumber().longValue(), 0),If(s2 IsNot Nothing, s2.getTimesUs().array().maxNumber().longValue(), 0),If(s1 IsNot Nothing, s1.getTimesUs().array().stdNumber().doubleValue(), 0.0),If(s2 IsNot Nothing, s2.getTimesUs().array().stdNumber().doubleValue(), 0.0)))
			Next s

			Dim header As Boolean = False
			Dim headerFormat2 As String = Nothing
			Dim format3 As String = Nothing
			Dim toAppend As IList(Of String) = Nothing
			For Each s As String In (If(c.sortBy() = SortBy.PROFILE2_PC, p1.Second.Keys, p2.Second.Keys))

				If Not set.Contains(s) Then
					Dim m As IDictionary(Of String, OpStats) = If(c.sortBy() = SortBy.PROFILE2_PC, p1.Second, p2.Second)
					Dim st As OpStats = m(s)
					If c.filter() IsNot Nothing Then
						Dim other As OpStats = If(c.sortBy() = SortBy.PROFILE2_PC, p1.Second(s), p2.Second(s))
						Dim keep As Boolean = c.filter().apply(other, st)
						If Not keep Then
							Continue For
						End If
					End If

					If Not header Then
						toAppend = New List(Of String)()

						longestName = 30
						longestOpName = 30
						For Each s2 As String In m.Keys
							longestName = Math.Max(longestName, s2.Length+1)
							longestOpName = Math.Max(longestOpName, m(s2).getOpName().length()+1)
						Next s2
						If c.format() Is Nothing OrElse c.format() = OutputFormat.TEXT Then
							headerFormat2 = "%-" & longestName & "s%-" & longestOpName & "s%-10s%-10s%-10s%-10s%-10s%-10s" & vbLf
							format3 = "%-" & longestName & "s%-" & longestOpName & "s%-10d%-10d%-10.2f%-10d%-10d%-10.2f" & vbLf
						Else
							headerFormat2 = "%s,%s,%s,%s,%s,%s,%s,%s" & vbLf
							format3 = "%s,%s,%d,%d,%.2f,%d,%d,%.2f" & vbLf
						End If

						sb.Append(" *** Operations not in profile ").Append(If(c.sortBy() = SortBy.PROFILE2_PC, "1", "2")).Append(" but in profile ").Append(If(c.sortBy() = SortBy.PROFILE2_PC, "2", "1")).Append(" ***" & vbLf)
						sb.Append(String.format(headerFormat2, "Op Name", "Op", "Count", "Total uS", "%", "Min", "Max", "Std"))
						header = True
					End If
					Dim allOpsUs As Long = If(c.sortBy() = SortBy.PROFILE2_PC, p1.First, p2.First)
					Dim pc As Double = (100.0 * st.getTimesUs().array().sumNumber().longValue()) / allOpsUs
					Dim arr As INDArray = st.getTimesUs().array()
					Dim min As Long = arr.minNumber().longValue()
					Dim max As Long = arr.maxNumber().longValue()
					Dim std As Double = arr.stdNumber().doubleValue()
					toAppend.Add(String.format(format3, s, st.getOpName(), st.getCount(), st.getSumUs(), pc, min, max, std))
				End If
			Next s
			If toAppend IsNot Nothing Then
				toAppend.Sort()
				For Each s As String In toAppend
					sb.Append(s)
				Next s
			End If

			Return sb.ToString()
		End Function

		Private Class ComparatorAnonymousInnerClass2
			Implements IComparer(Of String)

			Private c As org.nd4j.autodiff.listeners.profiler.comparison.Config
			Private p1 As Pair(Of Long, IDictionary(Of String, OpStats))
			Private p2 As Pair(Of Long, IDictionary(Of String, OpStats))

			Public Sub New(ByVal c As org.nd4j.autodiff.listeners.profiler.comparison.Config, ByVal p1 As Pair(Of Long, IDictionary(Of String, OpStats)), ByVal p2 As Pair(Of Long, IDictionary(Of String, OpStats)))
				Me.c = c
				Me.p1 = p1
				Me.p2 = p2
			End Sub

			Public Function Compare(ByVal o1 As String, ByVal o2 As String) As Integer Implements IComparer(Of String).Compare
				Select Case c.sortBy()
					Case PROFILE1_PC
						Return -Long.compare(p1.Second(o1).getSumUs(), p1.Second(o2).getSumUs())
					Case PROFILE2_PC
						Return -Long.compare(p2.Second(o1).getSumUs(), p2.Second(o2).getSumUs())
					Case RATIO
						Dim m1a As Double = meanTime(p1, o1)
						Dim m1b As Double = meanTime(p1, o2)
						Dim m2a As Double = meanTime(p2, o1)
						Dim m2b As Double = meanTime(p2, o2)
						Dim ratio1 As Double = m1a / m2a
						Dim ratio2 As Double = m1b / m2b
						Return -ratio1.CompareTo(ratio2)
					Case Else
						Throw New Exception()
				End Select
			End Function
		End Class

		Private Shared Function meanTime(ByVal p As Pair(Of Long, IDictionary(Of String, OpStats)), ByVal name As String) As Double
			If Not p.Second.ContainsKey(name) Then
				Return 0.0
			End If
			Return p.Second(name).getTimesUs().array().meanNumber().doubleValue()
		End Function


		Private Shared TF_PROFILE_ALIASES As IDictionary(Of String, String) = New Dictionary(Of String, String)()

		Shared Sub New()
			TF_PROFILE_ALIASES("_MklSoftmax") = "Softmax"
		End Sub

	End Class

End Namespace