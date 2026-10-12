Imports System
Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports ArrayUtils = org.apache.commons.lang3.ArrayUtils
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports Loss = org.nd4j.autodiff.listeners.Loss
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports Phase = org.nd4j.autodiff.listeners.profiler.data.Phase
Imports TraceEvent = org.nd4j.autodiff.listeners.profiler.data.TraceEvent
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports AtomicBoolean = org.nd4j.common.primitives.AtomicBoolean
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature

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
Namespace org.nd4j.autodiff.listeners.profiler


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Slf4j public class ProfilingListener extends org.nd4j.autodiff.listeners.BaseListener
	Public Class ProfilingListener
		Inherits BaseListener

		Private ReadOnly outputFile As File
		Private ReadOnly all As Boolean
		Private ReadOnly warmup As Integer
		Private ReadOnly nIter As Integer
		Private ReadOnly nMs As Long
		Private ReadOnly operations() As Operation

		Private ReadOnly pid As Long
		Private ReadOnly tid As Long
		Private firstOpStart As Long? = Nothing 'Used for time termination
		Private countTotalIter As Integer = 0
		Private logActive As Boolean = False
		Private opStartNano As Long

		Private writer As Writer
		Private json As ObjectMapper

		Private ReadOnly fileWritingThread As Thread
		Private ReadOnly writeQueue As BlockingQueue(Of TraceEvent)
		Private ReadOnly writing As New AtomicBoolean(False)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected ProfilingListener(@NonNull File outputFile, boolean all, int warmup, int nIter, long nMs, org.nd4j.autodiff.listeners.Operation[] operations)
		Protected Friend Sub New(ByVal outputFile As File, ByVal all As Boolean, ByVal warmup As Integer, ByVal nIter As Integer, ByVal nMs As Long, ByVal operations() As Operation)
			Preconditions.checkArgument(Not outputFile.exists(), "Output file already exists: %s", outputFile)
			Me.outputFile = outputFile
			Me.all = all
			Me.warmup = warmup
			Me.nIter = nIter
			Me.nMs = nMs
			Me.operations = operations

			Me.pid = ProcessId
			Me.tid = Thread.CurrentThread.getId()

			Try
				Me.writer = New StreamWriter(New StreamWriter(outputFile, False))
				Me.writer.write("[") 'JSON array open (array close is optional for Chrome profiler format)
			Catch e As IOException
				Throw New Exception(e)
			End Try

			Me.json = jsonMapper()

			'Set up a queue so file access doesn't add latency to the execution thread
			writeQueue = New LinkedBlockingDeque(Of TraceEvent)()
			fileWritingThread = New Thread(New RunnableAnonymousInnerClass(Me))
			fileWritingThread.setDaemon(True)
			fileWritingThread.Start()
		End Sub

		Private Class RunnableAnonymousInnerClass
			Implements ThreadStart

			Private ReadOnly outerInstance As ProfilingListener

			Public Sub New(ByVal outerInstance As ProfilingListener)
				Me.outerInstance = outerInstance
			End Sub

			Public Sub run()
				Try
					runHelper()
				Catch t As Exception
					log.error("Error when attempting to write results to file", t)
				End Try
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void runHelper() throws Exception
			Public Sub runHelper()
				Do
					Dim te As TraceEvent = outerInstance.writeQueue.take() 'Blocking
					outerInstance.writing.set(True)
					Try
						Dim j As String = outerInstance.json.writeValueAsString(te)
						outerInstance.writer.append(j)
						outerInstance.writer.append("," & vbLf)
					Catch e As IOException
						Throw New Exception(e)
					Finally
						outerInstance.writing.set(False)
					End Try
				Loop
			End Sub
		End Class

		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return operations Is Nothing OrElse ArrayUtils.contains(operations, operation)
		End Function

		Public Overrides Sub operationStart(ByVal sd As SameDiff, ByVal op As Operation)
			Me.logActive = operations Is Nothing OrElse ArrayUtils.contains(operations, op)
		End Sub

		Public Overrides Sub operationEnd(ByVal sd As SameDiff, ByVal op As Operation)
			If Me.logActive Then
				Do While (Not writeQueue.isEmpty() OrElse writing.get()) AndAlso fileWritingThread.IsAlive
					'Wait for file writing thread to catch up
					Try
						Thread.Sleep(100)
					Catch e As InterruptedException
						Throw New Exception(e)
					End Try
				Loop
				Try
					writer.flush()
				Catch e As IOException
					Throw New Exception(e)
				End Try
			End If
			Me.logActive = False
			If op = Operation.INFERENCE Then
				'Increment for inference; iteration done is called only for TRAINING
				countTotalIter += 1
			End If
		End Sub

		Public Overrides Sub iterationDone(ByVal sd As SameDiff, ByVal at As At, ByVal dataSet As MultiDataSet, ByVal loss As Loss)
			'Increment for training
			If logActive Then
				countTotalIter += 1
			End If
		End Sub

		Public Overrides Sub preOpExecution(ByVal sd As SameDiff, ByVal at As At, ByVal op As SameDiffOp, ByVal opContext As OpContext)
			If logActive Then
				opStartNano = System.nanoTime()

				If Not all AndAlso nMs > 0 AndAlso firstOpStart Is Nothing Then
					firstOpStart = opStartNano
				End If
			End If
		End Sub

		Public Overrides Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)
			If logActive Then
				Dim now As Long = System.nanoTime()

				If warmup > 0 AndAlso countTotalIter < warmup Then
					Return 'Skip due to warmup phase
				End If

				'Iteration termination
				Dim terminationPt As Integer = If(Me.nIter > 0, Me.nIter, Integer.MaxValue)
				If warmup > 0 AndAlso Me.nIter > 0 Then
					terminationPt += Me.warmup
				End If

				If countTotalIter > terminationPt Then
					logActive = False
					Return 'Skip due to max number of itertions
				End If

				'Time termination
'JAVA TO VB CONVERTER WARNING: Java to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
				If Not all AndAlso nMs > 0 AndAlso (now - firstOpStart.Value)/1000 > nMs Then
					logActive = False
					Return
				End If

				Dim [event] As TraceEvent = TraceEvent.builder().name(op.Op.opName()).categories(Collections.singletonList("Op")).ts(opStartNano \ 1000).dur((now - opStartNano) \ 1000).pid(CInt(pid)).tid(tid).ph(Phase.X).args(Collections.singletonMap(Of String, Object)("name", op.Name)).build()

				writeQueue.add([event])
			End If
		End Sub


		Private ReadOnly Property ProcessId As Long
			Get
				' Note: may fail in some JVM implementations
				' therefore fallback has to be provided
    
				' something like '<pid>@<hostname>', at least in SUN / Oracle JVMs
	'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
	'ORIGINAL LINE: final String jvmName = java.lang.management.ManagementFactory.getRuntimeMXBean().getName();
				Dim jvmName As String = ManagementFactory.getRuntimeMXBean().getName()
	'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
	'ORIGINAL LINE: final int index = jvmName.indexOf("@"c);
				Dim index As Integer = jvmName.IndexOf("@"c)
    
				If index < 1 Then
					' part before '@' empty (index = 0) / '@' not found (index = -1)
					Return 0
				End If
    
				Try
					Return Long.Parse(jvmName.Substring(0, index))
				Catch e As System.FormatException
					' ignore
				End Try
				Return 0
			End Get
		End Property

		''' <summary>
		''' Get a new JSON mapper for use in serializing/deserializing JSON format
		''' </summary>
		Public Shared Function jsonMapper() As ObjectMapper
			Dim json As New ObjectMapper()
			json.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			json.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			json.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, False)
			json.disable(SerializationFeature.INDENT_OUTPUT) 'One line

			Return json
		End Function

		''' <summary>
		''' Create a new builder </summary>
		''' <param name="outputFile"> Output file. Will be overwritten if file already exists </param>
		Public Shared Function builder(ByVal outputFile As File) As Builder
			Return New Builder(outputFile)
		End Function

		Public Class Builder
			Friend ReadOnly outputFile As File
			Friend all As Boolean = True
'JAVA TO VB CONVERTER NOTE: The field warmup was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend warmup_Conflict As Integer = 0
			Friend nIter As Integer = -1
			Friend nMs As Long = -1
'JAVA TO VB CONVERTER NOTE: The field operations was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend operations_Conflict() As Operation

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder(@NonNull File outputFile)
			Public Sub New(ByVal outputFile As File)
				Me.outputFile = outputFile
			End Sub

			''' <summary>
			''' If called, all data will be profiled with no limits (other than a warmup, if set)
			''' </summary>
			Public Overridable Function recordAll() As Builder
				Me.all = True
				Me.nIter = -1
				Me.nMs = -1
				Return Me
			End Function

			''' <summary>
			''' Specify the number of warmup iterations - i.e., these will be excluded from profiling results
			''' </summary>
			Public Overridable Function warmup(ByVal iterations As Integer) As Builder
				Me.warmup_Conflict = iterations
				Return Me
			End Function

			''' <summary>
			''' Set a limit on the maximum number of iterations to profile (after warmup, if any).
			''' Any ops executed after the specified number of iterations will not be profiled/recorded
			''' </summary>
			Public Overridable Function maxProfileIterations(ByVal iterations As Integer) As Builder
				Me.nIter = iterations
				Me.all = False
				Return Me
			End Function

			''' <summary>
			''' Set a limit on the maximum duration for profiling, in milliseconds.
			''' Any ops executed after the specified amount of time since the first (non-warmup) operation start will not be
			''' profiled/recorded
			''' </summary>
			Public Overridable Function maxProfilerMilliseconds(ByVal ms As Long) As Builder
				Me.nMs = ms
				Me.all = False
				Return Me
			End Function

			''' <summary>
			''' Specify the operations (training, inference, etc) to profile.
			''' If not set, all operations are profiled
			''' </summary>
'JAVA TO VB CONVERTER NOTE: The parameter operations was renamed since Visual Basic will not allow parameters with the same name as their enclosing function or property:
			Public Overridable Function operations(ParamArray ByVal operations_Conflict() As Operation) As Builder
				Me.operations_Conflict = operations_Conflict
				Return Me
			End Function

			''' <summary>
			''' Create the profiling listener
			''' </summary>
			Public Overridable Function build() As ProfilingListener
				Return New ProfilingListener(outputFile, all, warmup_Conflict, nIter, nMs, operations_Conflict)
			End Function
		End Class
	End Class

End Namespace