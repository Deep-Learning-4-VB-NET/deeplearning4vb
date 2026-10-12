Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
Imports FileUtils = org.apache.commons.io.FileUtils
Imports ExecPrintListener = org.nd4j.imports.listeners.ExecPrintListener
Imports OpExecOrderListener = org.nd4j.imports.tfgraphs.listener.OpExecOrderListener
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports IOUtils = org.apache.commons.io.IOUtils
Imports NumberUtils = org.apache.commons.lang3.math.NumberUtils
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports NativeGraphExecutioner = org.nd4j.autodiff.execution.NativeGraphExecutioner
Imports ExecutionMode = org.nd4j.autodiff.execution.conf.ExecutionMode
Imports ExecutorConfiguration = org.nd4j.autodiff.execution.conf.ExecutorConfiguration
Imports OutputMode = org.nd4j.autodiff.execution.conf.OutputMode
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports ArrayCloseMemoryMgr = org.nd4j.autodiff.samediff.internal.memory.ArrayCloseMemoryMgr
Imports CloseValidationMemoryMgr = org.nd4j.autodiff.samediff.internal.memory.CloseValidationMemoryMgr
Imports OpValidation = org.nd4j.autodiff.validation.OpValidation
Imports TestCase = org.nd4j.autodiff.validation.TestCase
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.common.function
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports org.nd4j.common.primitives
Imports ResourceFile = org.nd4j.common.resources.strumpf.ResourceFile
Imports StrumpfResolver = org.nd4j.common.resources.strumpf.StrumpfResolver
Imports ArrayUtil = org.nd4j.common.util.ArrayUtil
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports ArrayOptionsHelper = org.nd4j.linalg.api.shape.options.ArrayOptionsHelper
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports NDArrayStrings = org.nd4j.linalg.string.NDArrayStrings
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports TensorflowFrameworkImporter = org.nd4j.samediff.frameworkimport.tensorflow.importer.TensorflowFrameworkImporter
Imports Files = org.nd4j.shade.guava.io.Files
Imports FileSystemResource = org.springframework.core.io.FileSystemResource
Imports Resource = org.springframework.core.io.Resource
Imports PathMatchingResourcePatternResolver = org.springframework.core.io.support.PathMatchingResourcePatternResolver
Imports ResourcePatternResolver = org.springframework.core.io.support.ResourcePatternResolver
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports org.junit.jupiter.api.Assertions
import static org.nd4j.imports.tfgraphs.TFGraphsSkipNodes.skipNode

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

Namespace org.nd4j.imports.tfgraphs


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class TFGraphTestAllHelper
	Public Class TFGraphTestAllHelper
		Public Const resourceFolderVar As String = "DL4J_TEST_RESOURCES"
		Public Shared tensorflowFrameworkImporter As New TensorflowFrameworkImporter()
		Public Const PRINT_GRAPH_PROP As String = "org.nd4j.imports.tfgraphs.printgraphs"
		Public Enum ExecuteWith
			SAMEDIFF
			LIBND4J
			JUST_PRINT
		End Enum

		Public Class DefaultGraphLoader
			Implements BiFunction(Of File, String, SameDiff)

			Public Overridable Function apply(ByVal file As File, ByVal name As String) As SameDiff

				Dim prop As String = System.getProperty(PRINT_GRAPH_PROP,"false")
				Dim printGraph As Boolean? = Boolean.Parse(prop)
				If printGraph Then
					Try
						Dim graphDef As GraphDef = GraphDef.parseFrom(Files.toByteArray(file))
						Console.WriteLine("Processing graph : " & vbLf & graphDef)
					Catch e As IOException
						Console.WriteLine(e.ToString())
						Console.Write(e.StackTrace)
					End Try
				Else
					Console.WriteLine("Processing graph at path : " & vbLf & file.getAbsolutePath())
				End If

				Return tensorflowFrameworkImporter.runImport(file.getAbsolutePath(),java.util.Collections.emptyMap())
			End Function
		End Class

		Public Shared ReadOnly LOADER As New DefaultGraphLoader()


		Private Shared configuration As ExecutorConfiguration = ExecutorConfiguration.builder().executionMode(ExecutionMode.SEQUENTIAL).profilingMode(OpExecutioner.ProfilingMode.DISABLED).gatherTimings(True).outputMode(OutputMode.VARIABLE_SPACE).build()

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static List<Object[]> fetchTestParams(String baseDir, String modelFileName, ExecuteWith executeWith, File localTestDir) throws IOException
		Protected Friend Shared Function fetchTestParams(ByVal baseDir As String, ByVal modelFileName As String, ByVal executeWith As ExecuteWith, ByVal localTestDir As File) As IList(Of Object())
			Dim modelNames() As String = modelDirNames(baseDir, executeWith, modelFileName)
			Dim modelParams As IList(Of Object()) = New List(Of Object())()
			For i As Integer = 0 To modelNames.Length - 1
				Dim currentParams(3) As Object
				currentParams(0) = inputVars(modelNames(i), baseDir, localTestDir) 'input variable map - could be null
				currentParams(1) = outputVars(modelNames(i), baseDir, localTestDir) 'saved off predictions
				currentParams(2) = modelNames(i)
				currentParams(3) = localTestDir
				modelParams.Add(currentParams)
			Next i
			Return modelParams
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static void checkOnlyOutput(Map<String, org.nd4j.linalg.api.ndarray.INDArray> inputs, Map<String, org.nd4j.linalg.api.ndarray.INDArray> predictions, String modelName, String baseDir, String modelFilename, ExecuteWith execType, org.nd4j.common.function.BiFunction<File,String,org.nd4j.autodiff.samediff.SameDiff> loader, System.Nullable<Double> maxRelErrorOverride, System.Nullable<Double> minAbsErrorOverride, boolean printArraysDebugging) throws IOException
		Protected Friend Shared Sub checkOnlyOutput(ByVal inputs As IDictionary(Of String, INDArray), ByVal predictions As IDictionary(Of String, INDArray), ByVal modelName As String, ByVal baseDir As String, ByVal modelFilename As String, ByVal execType As ExecuteWith, ByVal loader As BiFunction(Of File, String, SameDiff), ByVal maxRelErrorOverride As Double?, ByVal minAbsErrorOverride As Double?, ByVal printArraysDebugging As Boolean)
			Preconditions.checkArgument((maxRelErrorOverride Is Nothing) = (minAbsErrorOverride Is Nothing), "Both maxRelErrorOverride and minAbsErrorOverride" & " must be null or both must be provided")
			Nd4j.EPS_THRESHOLD = 1e-3

			Dim outputsToCheck As ISet(Of String) = New HashSet(Of String)()
			For Each s As String In predictions.Keys
				' we need to convert name from python name format with . on indices, to :. i.e.: output.1 -> output:1
				If s.matches(".*\.\d+") Then
					Dim idx As Integer = s.LastIndexOf("."c)
					s = s.Substring(0, idx) & ":" & s.Substring(idx + 1)
				End If
				outputsToCheck.Add(s)
			Next s

			Dim p As Pair(Of SameDiff, IDictionary(Of String, INDArray)) = getGraphAfterExec(baseDir, modelFilename, modelName, inputs, execType, loader, Nothing, outputsToCheck, printArraysDebugging)
			Dim graph As SameDiff = p.First
			Dim sameDiffPredictions As IDictionary(Of String, INDArray) = p.Second
	'        SameDiff graph = graphLoaderFunction.apply(new ClassPathResource(baseDir + "/" + modelName + "/" + modelFilename).getFile(), modelName);
			'Collect coverage info about ops
	'        TensorflowFrameworkImporter tensorflowFrameworkImporter = new TensorflowFrameworkImporter();
	'        File oldModel = new ClassPathResource(baseDir + "/" + modelName + "/" + modelFilename).getFile();
	'        GraphDef g  = GraphDef.parseFrom(IOUtils.toByteArray(oldModel.toURI()));
	'        TensorflowIRGraph tensorflowIRGraph = new TensorflowIRGraph(g,tensorflowFrameworkImporter.getOpDefList(),tensorflowFrameworkImporter.getRegistry());
	'        TensorflowIRGraphRunner tensorflowIRGraphRunner = new TensorflowIRGraphRunner(tensorflowIRGraph,p.getFirst().inputs(),outputsToCheck.stream().collect(Collectors.toList()));
	'        Map<String,INDArray> outputs = tensorflowIRGraphRunner.run(inputs);
	'        SameDiff oldForComparison = TFGraphMapper.importGraph(oldModel);
	'        Map<String,INDArray> oldOutputs = oldForComparison.outputAll(inputs);
	'        
			OpValidation.collectTensorflowImportCoverage(graph)

			If Not execType.Equals(ExecuteWith.JUST_PRINT) Then
				assertTrue(predictions.Keys.Count > 0,"No predictions to validate")
				For Each outputNode As String In predictions.Keys
					Dim nd4jPred As INDArray = Nothing
					Dim tfPred As INDArray = Nothing

					Dim nd4jNode As String = outputNode

					' we need to convert name from python name format with . on indices, to :. i.e.: output.1 -> output:1
					If outputNode.Contains(".") Then
						nd4jNode = outputNode.replaceAll("\.", ":")
					End If

					Try
						nd4jPred = sameDiffPredictions(nd4jNode)
					Catch e As System.NullReferenceException
						Throw New System.NullReferenceException("Can't find SameDiff variable with name [" & nd4jNode & "]")
					End Try

					Try
						tfPred = predictions(outputNode)
					Catch e As System.NullReferenceException
						Throw New System.NullReferenceException("Can't find predicted variable with name [" & outputNode & "]")
					End Try

					assertNotNull(nd4jPred)
					assertNotNull(tfPred)

					If maxRelErrorOverride Is Nothing Then
						Dim sTf() As Long = tfPred.shape()
						Dim sNd4j() As Long = nd4jPred.shape()
						assertArrayEquals(sTf, sNd4j,"Shapes for node """ & outputNode & """ are not equal: TF: " & java.util.Arrays.toString(sTf) & " vs SD: " & java.util.Arrays.toString(sNd4j))

						' TODO: once we add more dtypes files - this should be removed
						If tfPred.dataType() <> nd4jPred.dataType() Then
							nd4jPred = nd4jPred.castTo(tfPred.dataType())
						End If

						Dim eq As Boolean = getEqualityFunction(modelName, outputNode, tfPred, nd4jPred).apply(tfPred, nd4jPred)

						If Not eq Then
							'Check for both NaN, both inf
							If tfPred.dataType().isFPType() AndAlso tfPred.equalShapes(nd4jPred) AndAlso tfPred.NaN.castTo(DataType.INT).sumNumber().intValue() = tfPred.length() AndAlso nd4jPred.NaN.castTo(DataType.INT).sumNumber().intValue() = nd4jPred.length() Then
								'All NaNs in both arrays
								eq = True
							ElseIf tfPred.dataType().isFPType() AndAlso tfPred.equalShapes(nd4jPred) AndAlso tfPred.Infinite.castTo(DataType.INT).sumNumber().intValue() = tfPred.length() AndAlso nd4jPred.Infinite.castTo(DataType.INT).sumNumber().intValue() = nd4jPred.length() Then
								'All infinite in both arrays. But need to check that it's all positive vs. negative infinite in both cases...
								Dim iter As New NdIndexIterator(tfPred.shape())
								eq = True
								Do While iter.MoveNext()
									Dim [next]() As Long = iter.Current
									'Already know they are both infinite, only question is whether they are both positive and negative
									Dim d1 As Double = tfPred.getDouble([next])
									Dim d2 As Double = nd4jPred.getDouble([next])
									If (d1 > 0) <> (d2 > 0) Then
										eq = False
										Exit Do
									End If
								Loop
							End If

							If Not eq Then
								Dim s As New NDArrayStrings()
								Dim s1 As String = s.format(tfPred, False)
								Dim s2 As String = s.format(nd4jPred, False)
								Console.Write("TF: ")
								Console.WriteLine(tfPred.toStringFull())
								Console.Write("SD: ")
								Console.WriteLine(nd4jPred.toStringFull())
							End If
						End If

						assertTrue(eq,"Predictions do not match on " & modelName & ", node " & outputNode)
					Else

						If Not tfPred.equalShapes(nd4jPred) Then
							fail("Output node """ & outputNode & """ SameDiff output shape does not match TF output shape: SameDiff shape: " & java.util.Arrays.toString(nd4jPred.shape()) & " vs. TF shape: " & java.util.Arrays.toString(tfPred.shape()))
						End If

						If tfPred.dataType() <> nd4jPred.dataType() Then
							fail("Output node """ & outputNode & """ SameDiff output datatype does not match TF output : SameDiff type: " & nd4jPred.dataType() & " vs. TF datatype: " & tfPred.dataType())
						End If

						If Not tfPred.dataType().isFPType() Then
							'Can't do relative error on long type...
							tfPred = tfPred.castTo(DataType.DOUBLE)
							nd4jPred = nd4jPred.castTo(DataType.DOUBLE)
						End If

						Dim diff As INDArray = Transforms.abs(tfPred.sub(nd4jPred), False)
						Dim absErrorMask As INDArray = diff.gte(minAbsErrorOverride).castTo(tfPred.dataType()) 'value 1 if x[i] > minAbsError; value 0 otherwise. Used to get rid of 1e-30 vs. 1e-29 type failures
						Dim sumAbs As INDArray = Transforms.abs(tfPred, True).addi(Transforms.abs(nd4jPred, True))
						BooleanIndexing.replaceWhere(sumAbs, 1.0, Conditions.equals(0.0)) 'Can only get 0.0 if both are zeros - need to avoid 0/0=NaN
						Dim relError As INDArray = diff.divi(sumAbs)
						relError.muli(absErrorMask)


	'                    
	'                    Try to detect bad test.
	'                    The idea: suppose all values are small, and are excluded due to minAbsError threshold
	'                    i.e., all 1e-5 vs. -1e-5 with min abs error of 1e-4
	'                    
						'TODO FIX ME
						Dim maxAbs As INDArray = Transforms.max(Transforms.abs(tfPred.castTo(DataType.DOUBLE), True), Transforms.abs(nd4jPred.castTo(DataType.DOUBLE), True), True)
						Dim countMaxAbsGTThreshold As Long = maxAbs.gte(minAbsErrorOverride).castTo(DataType.INT).sumNumber().intValue()
						Dim countNotMasked As Long = absErrorMask.sumNumber().intValue() 'Values are 0 or 1... if all 0s -> nothing being tested
						If countNotMasked = 0 AndAlso countMaxAbsGTThreshold = 0 Then
							fail("All values for node " & outputNode & " are masked out due to minAbsError=" & minAbsErrorOverride & " and max values are all less than minAbsError - nothing can be tested here")
						End If

						Dim countExceeds As Integer = Nd4j.Executioner.exec(New MatchCondition(relError, Conditions.greaterThan(maxRelErrorOverride))).getInt(0)

						Dim maxRE As Double = -1
						If countExceeds > 0 Then
							maxRE = relError.maxNumber().doubleValue()
						End If


						assertEquals(0, countExceeds,outputNode & ": " & countExceeds & " values exceed maxRelError=" & maxRelErrorOverride & " with minAbsError=" & minAbsErrorOverride & "; largest observed relError=" & maxRE)
					End If
				Next outputNode
				log.info("TEST {} PASSED with {} arrays compared...", modelName, predictions.Keys.Count)
			End If

			'Serialize and deserialize, check equality:
			Dim serialized As ByteBuffer = graph.asFlatBuffers(True)
			Preconditions.checkNotNull(serialized, "Serialization failed? Null output")
			OpValidation.checkDeserializedEquality(graph, serialized, (New TestCase(graph)).testName(modelName).placeholderValues(inputs))


			Nd4j.EPS_THRESHOLD = 1e-5
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void checkIntermediate(Map<String, org.nd4j.linalg.api.ndarray.INDArray> inputs, String modelName, String baseDir, String modelFileName, ExecuteWith execType, File localTestDir, boolean printArraysDebugging) throws IOException
		Public Shared Sub checkIntermediate(ByVal inputs As IDictionary(Of String, INDArray), ByVal modelName As String, ByVal baseDir As String, ByVal modelFileName As String, ByVal execType As ExecuteWith, ByVal localTestDir As File, ByVal printArraysDebugging As Boolean)
			checkIntermediate(inputs, modelName, baseDir, modelFileName, execType, LOADER, Nothing, Nothing, localTestDir, printArraysDebugging)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void checkIntermediate(Map<String, org.nd4j.linalg.api.ndarray.INDArray> inputs, String modelName, String baseDir, String modelFileName, ExecuteWith execType, org.nd4j.common.function.BiFunction<File,String,org.nd4j.autodiff.samediff.SameDiff> loader, System.Nullable<Double> maxRelErrorOverride, System.Nullable<Double> minAbsErrorOverride, File localTestDir, boolean printArraysDebugging) throws IOException
		Public Shared Sub checkIntermediate(ByVal inputs As IDictionary(Of String, INDArray), ByVal modelName As String, ByVal baseDir As String, ByVal modelFileName As String, ByVal execType As ExecuteWith, ByVal loader As BiFunction(Of File, String, SameDiff), ByVal maxRelErrorOverride As Double?, ByVal minAbsErrorOverride As Double?, ByVal localTestDir As File, ByVal printArraysDebugging As Boolean)
			Preconditions.checkArgument((maxRelErrorOverride Is Nothing) = (minAbsErrorOverride Is Nothing), "Both maxRelErrorOverride and minAbsErrorOverride" & " must be null or both must be provided")
			Nd4j.EPS_THRESHOLD = 1e-3
			Dim listener As New OpExecOrderListener() 'Used to collect exec order
			Dim p As Pair(Of SameDiff, IDictionary(Of String, INDArray)) = getGraphAfterExec(baseDir, modelFileName, modelName, inputs, execType, loader, Collections.singletonList(listener), Nothing, printArraysDebugging)
			Dim graph As SameDiff = p.First
			Dim sdPredictions As IDictionary(Of String, INDArray) = p.Second

			'Collect coverage info about ops
			OpValidation.collectTensorflowImportCoverage(graph)

			If Not execType.Equals(ExecuteWith.JUST_PRINT) Then
				Dim count As Integer = 0
				'Evaluate the nodes in their execution order - this is useful for debugging (as we want the *first* failure
				' to be detected before later failures)
				Dim varNames As IList(Of String) = New List(Of String)()
				Dim fns As IDictionary(Of String, SameDiffOp) = graph.getOps()
				Dim execOrder As IList(Of String) = listener.getOpNamesList()
				For Each opName As String In execOrder
					Dim outputs() As String = graph.getOutputsForOp(fns(opName).getOp())
					Collections.addAll(varNames, outputs)
				Next opName

				For Each varName As String In varNames
					If Not inputs.ContainsKey(varName) Then 'avoiding placeholders
						Dim tfValue As INDArray = intermediateVars(modelName, baseDir, varName, localTestDir)
						If tfValue Is Nothing Then
							Continue For
						End If
						log.info("Starting check: variable {}", varName)
						If skipNode(modelName, varName) Then
							log.info(vbLf & vbTab & "FORCING no check on " & varName)
						Else
							'assertArrayEquals("Shape not equal on node " + varName, tfValue.shape(), graph.getVariable(varName).getShape());
							Dim sdVal As INDArray = sdPredictions(varName)
							If maxRelErrorOverride IsNot Nothing Then
								Dim diff As INDArray = Transforms.abs(tfValue.sub(sdVal), False)
								Dim absErrorMask As INDArray = diff.gte(minAbsErrorOverride) 'value 1 if x[i] > minAbsError; value 0 otherwise. Used to get rid of 1e-30 vs. 1e-29 type failures
								Dim sumAbs As INDArray = Transforms.abs(tfValue, True).addi(Transforms.abs(sdVal, True))
								BooleanIndexing.replaceWhere(sumAbs, 1.0, Conditions.equals(0.0)) 'Can only get 0.0 if both are zeros - need to avoid 0/0=NaN
								Dim relError As INDArray = diff.divi(sumAbs)
								relError.muli(absErrorMask)

								Dim countExceeds As Integer = Nd4j.Executioner.exec(New MatchCondition(relError, Conditions.greaterThan(maxRelErrorOverride))).getInt(0)

								Dim maxRE As Double = -1
								'Mainly used for analysis in debugger:
								Dim op As DifferentialFunction = Nothing
								Dim opInputs() As String = Nothing
								If countExceeds > 0 Then
									maxRE = relError.maxNumber().doubleValue()
									'Find the op that this variable is produced by
									op = graph.getVariableOutputOp(varName)
									opInputs = graph.getInputsForOp(op)
								End If


								assertEquals(0, countExceeds,varName & ": " & countExceeds & " values exceed maxRelError=" & maxRelErrorOverride & " with minAbsError=" & minAbsErrorOverride & "; largest observed relError=" & maxRE)
							Else
	'                            assertEquals("Value not equal on node " + varName, tfValue, sdVal);
								If tfValue.Equals(sdVal) Then
									Console.WriteLine("Pass: " & varName)
								Else
									Console.WriteLine("FAIL: " & varName)
									Console.WriteLine("TF:" & vbLf & tfValue)
									Console.WriteLine("SD:" & vbLf & sdVal)
								End If

							End If
							log.info("Values and shapes equal for {}", varName)
							count += 1
						End If

					End If
				Next varName

				assertTrue(count > 0,"No intermediate variables were checked")
			End If

			Nd4j.EPS_THRESHOLD = 1e-5
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static org.nd4j.common.primitives.Pair<org.nd4j.autodiff.samediff.SameDiff, Map<String,org.nd4j.linalg.api.ndarray.INDArray>> getGraphAfterExec(String baseDir, String modelFilename, String modelName, Map<String, org.nd4j.linalg.api.ndarray.INDArray> inputs, ExecuteWith executeWith, org.nd4j.common.function.BiFunction<File,String,org.nd4j.autodiff.samediff.SameDiff> graphLoaderFunction, List<org.nd4j.autodiff.listeners.Listener> listeners, @Set<String> requiredOutputs, boolean printArraysDebugging) throws IOException
		Public Shared Function getGraphAfterExec(ByVal baseDir As String, ByVal modelFilename As String, ByVal modelName As String, ByVal inputs As IDictionary(Of String, INDArray), ByVal executeWith As ExecuteWith, ByVal graphLoaderFunction As BiFunction(Of File, String, SameDiff), ByVal listeners As IList(Of Listener), ByVal requiredOutputs As ISet(Of String), ByVal printArraysDebugging As Boolean) As Pair(Of SameDiff, IDictionary(Of String, INDArray))
			log.info("RUNNING TEST {}...", modelName)
			Dim graph As SameDiff = graphLoaderFunction.apply((New ClassPathResource(baseDir & "/" & modelName & "/" & modelFilename)).File, modelName)
			If listeners IsNot Nothing Then
				graph.setListeners(listeners)
			End If

			If printArraysDebugging Then
				graph.addListeners(New ExecPrintListener())
			End If

			If requiredOutputs Is Nothing Then
				requiredOutputs = graph.variableMap().Keys
			End If

			Dim outMap As IDictionary(Of String, INDArray) = Nothing
			If executeWith.Equals(ExecuteWith.SAMEDIFF) Then
				'Set memory manager - check that all arrays (other than the ones we requested as output)
				Dim mmgr As New CloseValidationMemoryMgr(graph, New ArrayCloseMemoryMgr())
	'            long tid = Thread.currentThread().getId();
	'            if(!graph.getSessions().containsKey(tid))
	'                graph.getSessions().put(tid, new InferenceSession(graph));
				'Execute
				' graph.getSessions().get(tid).setMmgr(mmgr);
				Dim shapes As IDictionary(Of String, String) = New Dictionary(Of String, String)()
				inputs.SetOfKeyValuePairs().ForEach(Sub(entry)
				shapes(entry.getKey()) = java.util.Arrays.toString(entry.getValue().shape())
				End Sub)

				log.info("Testing inputs with names " & inputs.Keys & " and shapes " & shapes)

				outMap = graph.output(inputs, New List(Of )(requiredOutputs))

				'Check that all arrays were released
				'mmgr.assertAllReleasedExcept(outMap.values());
				graph.getSessions().clear()
			ElseIf executeWith.Equals(ExecuteWith.LIBND4J) Then
				For Each input As String In inputs.Keys
					graph.associateArrayWithVariable(inputs(input), graph.variableMap()(input))
				Next input

	'            val string = graph.asFlatPrint();
	'            log.info("Graph structure: \n{}", string);
				Dim executioner As val = New NativeGraphExecutioner()
				Dim results As val = executioner.executeGraph(graph, configuration)

			ElseIf executeWith.Equals(ExecuteWith.JUST_PRINT) Then
				For Each input As String In inputs.Keys
					graph.associateArrayWithVariable(inputs(input), graph.variableMap()(input))
				Next input

				Dim [string] As val = graph.asFlatPrint()
				log.info("Graph structure: " & vbLf & "{}", [string])
			End If

			Return New Pair(Of SameDiff, IDictionary(Of String, INDArray))(graph, outMap)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static String[] modelDirNames(String base_dir, ExecuteWith executeWith, String modelFileName) throws IOException
		Private Shared Function modelDirNames(ByVal base_dir As String, ByVal executeWith As ExecuteWith, ByVal modelFileName As String) As String()
			Dim resolver As ResourcePatternResolver = New PathMatchingResourcePatternResolver((New ClassPathResource(base_dir)).ClassLoader)
			Dim resources() As Resource = resolver.getResources("classpath*:" & base_dir & "/**/" & modelFileName)
			Dim exampleNames(resources.Length - 1) As String
			For i As Integer = 0 To resources.Length - 1
				Dim nestedName As String = resources(i).getURL().ToString().Split(base_dir & "/", True)(1)
				exampleNames(i) = nestedName.replaceAll(Pattern.quote(base_dir), "").replaceAll("/" & modelFileName, "")
			Next i
			Return exampleNames
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static Map<String, org.nd4j.linalg.api.ndarray.INDArray> inputVars(String modelName, String base_dir, File localTestDir) throws IOException
		Protected Friend Shared Function inputVars(ByVal modelName As String, ByVal base_dir As String, ByVal localTestDir As File) As IDictionary(Of String, INDArray)
			Return readVars(modelName, base_dir, "**.placeholder", True, localTestDir)
		End Function


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static Map<String, org.nd4j.linalg.api.ndarray.INDArray> outputVars(String modelName, String base_dir, File localTestDir) throws IOException
		Protected Friend Shared Function outputVars(ByVal modelName As String, ByVal base_dir As String, ByVal localTestDir As File) As IDictionary(Of String, INDArray)
			Return readVars(modelName, base_dir, "**.prediction", True, localTestDir)
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static Map<String, org.nd4j.linalg.api.ndarray.INDArray> inbetweenVars(String modelName, String base_dir, File localTestDir) throws IOException
		Protected Friend Shared Function inbetweenVars(ByVal modelName As String, ByVal base_dir As String, ByVal localTestDir As File) As IDictionary(Of String, INDArray)
			Return readVars(modelName, base_dir, "**.prediction_inbw", True, localTestDir)
		End Function


		'return readVars(modelName, base_dir, "**.prediction_inbw", true);

		''' <summary>
		''' Possible for a single node to give multiple outputs
		''' 
		''' How is a node that has a list of outputs like in the case of "node_multiple_out" work
		''' Below is hardcoded for a single node
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static org.nd4j.linalg.api.ndarray.INDArray intermediateVars(String modelName, String base_dir, String varName, File localTestDir) throws IOException
		Protected Friend Shared Function intermediateVars(ByVal modelName As String, ByVal base_dir As String, ByVal varName As String, ByVal localTestDir As File) As INDArray
			'convert varName to convention used in naming files
			' "/" replaced by "____"; followed by a digit indicating the output number followed by prediction_inbw.(shape|csv)
			If varName.Contains(":") Then
				varName = varName.Replace(":"c, "."c)
			Else
				varName = varName & ".0"
			End If
			Dim name As String = varName.replaceAll("/", "____") & ".prediction_inbw"
			Dim nodeSepOutput As IDictionary(Of String, INDArray) = readVars(modelName, base_dir, name, True, localTestDir)

			Dim importNameWorkaround As Boolean = False
			If nodeSepOutput.Count = 0 Then
				'Edge case: intermediates were generated with help of import_graph_def method, which by default adds "import/" to names
				' for some reason. https://www.tensorflow.org/api_docs/python/tf/graph_util/import_graph_def
				'So many of earlier intermediate nodes test data were generated with filenames like "import___X..." instead of "X..."
				name = "import____" & name
				nodeSepOutput = readVars(modelName, base_dir, name, True, localTestDir)
				importNameWorkaround = True
			End If

			'required check for pattern matching as there are scopes and "*" above is a greedy match
			Dim removeList As ISet(Of String) = confirmPatternMatch(nodeSepOutput.Keys,If(importNameWorkaround, "import/" & varName, varName))
			For Each toRemove As String In removeList
				nodeSepOutput.Remove(toRemove)
			Next toRemove
			If importNameWorkaround Then
				Return nodeSepOutput("import/" & varName) 'this *should* return a list of the indarrays for each node
			Else
				Return nodeSepOutput(varName) 'this *should* return a list of the indarrays for each node
			End If
		End Function

		Public Shared Function confirmPatternMatch(ByVal setOfNames As ISet(Of String), ByVal varName As String) As ISet(Of String)
			Dim removeList As ISet(Of String) = New HashSet(Of String)()
			For Each name As String In setOfNames
				If name.Equals(varName) Then
					Continue For
				End If
				Dim splitByPeriod() As String = name.Split("\.", True)
				'not a number - maybe another variable deeper in the same scope
				If Not NumberUtils.isNumber(splitByPeriod(splitByPeriod.Length - 1)) Then
					removeList.Add(name)
				ElseIf Not String.join(".", Arrays.CopyOfRange(splitByPeriod, 0, splitByPeriod.Length - 1)).Equals(varName) Then
					removeList.Add(name)
				End If
			Next name
			Return removeList
		End Function


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected static Map<String, org.nd4j.linalg.api.ndarray.INDArray> readVars(String modelName, String base_dir, String pattern, boolean recursive, File localTestDir) throws IOException
		Protected Friend Shared Function readVars(ByVal modelName As String, ByVal base_dir As String, ByVal pattern As String, ByVal recursive As Boolean, ByVal localTestDir As File) As IDictionary(Of String, INDArray)
			Dim varMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			Dim modelDir As String = base_dir & "/" & modelName

			' key is variable name, value is data type
			Dim dtypes As val = New Dictionary(Of String, DataType)()

			Dim resources As IList(Of Pair(Of Resource, Resource)) = New List(Of Pair(Of Resource, Resource))()
			If recursive Then
				Dim nameRegex As String = pattern.Replace("**.",".*\.") & "\.shape"
				' checking out, if local folder declared
				Dim localPath As String = Environment.GetEnvironmentVariable(TFGraphTestAllHelper.resourceFolderVar)
				If localPath IsNot Nothing AndAlso (Not localPath.Contains("src/main/resources") AndAlso Not localPath.Contains("src\main\resources")) Then
					localPath = FilenameUtils.concat(localPath, "src/main/resources")
				End If


				' baseDir will differ, depending on run mode
				Dim baseDir As File = If(localPath Is Nothing, New File(localTestDir, "extracted/" & modelName), New File(localPath, base_dir & "/" & modelName))
				Dim arr() As String = baseDir.list()

				If Not baseDir.exists() OrElse arr Is Nothing OrElse arr.Length = 0 Then
					' we're skipping extraction if we're using local copy of dl4j-tests-resources
					If localPath Is Nothing Then
						baseDir.mkdirs()
						FileUtils.forceDeleteOnExit(baseDir)
						Dim md As String = modelDir
						If Not md.EndsWith("/", StringComparison.Ordinal) AndAlso Not md.EndsWith("\", StringComparison.Ordinal) Then
							md = md & "/"
						End If

						Call (New ClassPathResource(md)).copyDirectory(baseDir)
					Else
						Throw New System.InvalidOperationException("local directory declared but could not find files: " & baseDir.getAbsolutePath())
					End If

				End If

				Dim queue As New LinkedList(Of File)()
				queue.AddLast(baseDir)

				Do While queue.Count > 0
					Dim subdir As File = queue.RemoveFirst()
					Dim files() As File = subdir.listFiles()
					If files IsNot Nothing Then
						For Each f As File In files
							If f.isDirectory() Then
								queue.AddLast(f)
							Else
								Dim filename As String = f.getName()
								If filename.matches(nameRegex) Then
									Dim csvFile As New File(f.getAbsolutePath().replace(".shape",".csv"))
									resources.Add(New Pair(Of Resource, Resource)(New FileSystemResource(f), New FileSystemResource(csvFile)))
								ElseIf filename.Equals("dtypes") Then
									Dim stringList As IList(Of String)

									Try
											Using [is] As val = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
											stringList = IOUtils.readLines([is], StandardCharsets.UTF_8)
        
											For Each s As val In stringList
												Dim split As val = s.split("\ ")
        
												Dim okey As val = split(0).replaceAll("____", "/")
												' adopt / in names
												Dim key As val = modelDir & "/" & okey
        
												' parse type directly
												Dim value As DataType = ArrayOptionsHelper.dataType(split(1))
        
												' adding key directly
												'if (dtypes.containsKey(key))
												'    throw new ND4JIllegalStateException("Specified key already exist: [" + key + "]");
												'else
        
												dtypes.put(key, value)
        
												' adding zero output duplicate (if it doesn't exist)
												If key.endsWith(".0") Then
													Dim nkey As val = key.replaceAll("\.0$","")
													If Not dtypes.containsKey(nkey) Then
														dtypes.put(nkey, value)
													End If
												ElseIf key.endsWith(":0") Then
													Dim nkey As val = key.replaceAll(":0$","")
													If Not dtypes.containsKey(nkey) Then
														dtypes.put(nkey, value)
													End If
												End If
											Next s
											End Using
									Catch e As FileNotFoundException
										stringList = New List(Of String)()
									End Try
								End If
							End If
						Next f
					End If
				Loop
			Else
				Dim resolver As ResourcePatternResolver = New PathMatchingResourcePatternResolver((New ClassPathResource(modelDir)).ClassLoader)
				Dim r() As Resource = resolver.getResources("classpath*:" & modelDir & "/" & pattern & ".shape")
				For Each res As Resource In r
					Dim fileName As String = res.getFilename()
					Dim varPath As String = modelDir & "/" & fileName
					Dim r2 As Resource = New org.springframework.core.io.ClassPathResource(varPath.Replace(".shape", ".csv"))
					resources.Add(New Pair(Of Resource, Resource)(res, r2))
				Next res

			End If

	'        Preconditions.checkState(!dtypes.isEmpty(), "No datatypes file was found");


			For i As Integer = 0 To resources.Count - 1
				Dim u As URI = resources(i).getFirst().getURI()
				Dim varName As String = u.ToString()
				Dim idx As Integer = varName.IndexOf(modelName, StringComparison.Ordinal)
				varName = varName.Substring(idx + modelName.Length+1) '+1 for "/"
				varName = varName.replaceAll("____","/")
				varName = varName.replaceAll(".placeholder.shape","")
				varName = varName.replaceAll(".prediction.shape","")
				varName = varName.replaceAll(".prediction_inbw.shape","")

				Dim type As DataType = dtypes.get(modelDir & "/" & varName)

				Dim lines As IList(Of String) '= FileUtils.readLines(new ClassPathResource(varPath).getFile(), Charset.forName("UTF-8"));
				Using [is] As Stream = New BufferedInputStream(resources(i).getFirst().getInputStream())
					lines = IOUtils.readLines([is], StandardCharsets.UTF_8)
				End Using
				Dim filtered As IList(Of String) = New List(Of String)(lines.Count)
				For Each s As String In lines
					Dim trimmed As String = s.Trim()
					If trimmed.Length > 0 Then
						filtered.Add(trimmed)
					End If
				Next s

				If type = Nothing Then
					log.warn("DATATYPE NOT AVAILABLE FOR: {} - {}", modelName, varName)
					'Soon: this will be an exception
					type = DataType.FLOAT
				End If

				Dim varValue As INDArray
				If filtered.Count = 0 Then
					'Scalar
					Dim content As String = IOUtils.toString(resources(i).getSecond().getInputStream(), StandardCharsets.UTF_8)
					Select Case type.innerEnumValue
						Case DataType.InnerEnum.DOUBLE, FLOAT, HALF, BFLOAT16
							varValue = Nd4j.scalar(type, parseDouble(content))
						Case DataType.InnerEnum.LONG, INT, [SHORT], UBYTE, [BYTE], UINT16, UINT32, UINT64
							varValue = Nd4j.scalar(type, parseLong(content))
						Case DataType.InnerEnum.BOOL
							varValue = Nd4j.scalar(parseBoolean(content))
						Case DataType.InnerEnum.UTF8
							varValue = Nd4j.scalar(content)
						Case Else
							Throw New System.NotSupportedException("Unknown / not implemented datatype: " & type)
					End Select
				Else
					Dim varShape(filtered.Count - 1) As Integer
					For j As Integer = 0 To filtered.Count - 1
						varShape(j) = Integer.Parse(filtered(j))
					Next j

					Try
						Dim content As String
						Dim p As Pair(Of Resource, Resource) = resources(i)
						Dim isRef As Boolean = p.Second.isFile() AndAlso Not p.Second.exists()

						Dim stream As Stream
						If isRef Then
							'Slight hack for loading strumpf reference files
							Dim r As File = (New StrumpfResolver()).localCacheRoot()
							Dim path As String = p.Second.getFile() & StrumpfResolver.REF
							Dim f As File = ResourceFile.fromFile(path).localFile(r)
							stream = New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
						Else
							stream = New BufferedInputStream(resources(i).getSecond().getInputStream())
						End If

						Using [is] As Stream = stream
							content = String.join(vbLf, IOUtils.readLines([is], StandardCharsets.UTF_8))
						End Using

						If content.Length = 0 Then
							'Should be zeros in shape
							Dim foundZero As Boolean = False
							For Each s As Integer In varShape
								foundZero = foundZero Or (s = 0)
							Next s
							If foundZero Then
								varValue = Nd4j.create(type, ArrayUtil.toLongArray(varShape))
							Else
								Throw New System.InvalidOperationException("Empty data but non-empty shape: " & resources(i).getSecond())
							End If
						Else
							If varShape.Length = 1 AndAlso varShape(0) = 0 Then 'Annoyingly, some scalars have shape [0] instead of []
								varShape = New Integer(){}
							End If

							Dim cLines() As String = content.Split(vbLf, True)
							Select Case type.innerEnumValue
								Case DataType.InnerEnum.DOUBLE, FLOAT, HALF, BFLOAT16
									Dim dArr(cLines.Length - 1) As Double
									Dim x As Integer=0
									Do While x < dArr.Length
										dArr(x) = parseDouble(cLines(x))
										x += 1
									Loop
									varValue = Nd4j.createFromArray(dArr).castTo(type).reshape("c"c, varShape)
								Case DataType.InnerEnum.LONG, INT, [SHORT], UBYTE, [BYTE], UINT16, UINT32, UINT64
									Dim lArr(cLines.Length - 1) As Long
									Dim y As Integer=0
									Do While y < lArr.Length
										lArr(y) = parseLong(cLines(y))
										y += 1
									Loop
									varValue = Nd4j.createFromArray(lArr).castTo(type).reshape("c"c, varShape)
								Case DataType.InnerEnum.BOOL
									Dim bArr(cLines.Length - 1) As Boolean
									Dim z As Integer = 0
									Do While z < bArr.Length
										bArr(z) = parseBoolean(cLines(z))
										z += 1
									Loop
									varValue = Nd4j.createFromArray(bArr).reshape("c"c, varShape)
								Case DataType.InnerEnum.UTF8
									varValue = Nd4j.create(cLines).reshape("c"c, varShape)
								Case Else
									Throw New System.NotSupportedException("Unknown / not implemented datatype: " & type)
							End Select
						End If
					Catch e As System.FormatException
						log.warn("Error parsing number", e)
						Continue For
					End Try
				End If

				varMap(varName) = varValue
			Next i
			Return varMap
		End Function

		Private Shared Function parseLong(ByVal line As String) As Long
			line = line.Trim() 'Handle whitespace
			If line.matches("-?\d+\.0+") Then
				'Annoyingly, some integer data is stored with redundant/unnecessary zeros - like "-7.0000000"
				Return Long.Parse(line.Substring(0, line.IndexOf("."c)))
			Else
				Return Long.Parse(line)
			End If
		End Function

		Private Shared Function parseDouble(ByVal line As String) As Double
			line = line.Trim() 'Handle whitespace - some lines are like "      -inf"
			If "nan".Equals(line, StringComparison.OrdinalIgnoreCase) Then
				Return Double.NaN
			ElseIf "inf".Equals(line, StringComparison.OrdinalIgnoreCase) Then
				Return Double.PositiveInfinity
			ElseIf "-inf".Equals(line, StringComparison.OrdinalIgnoreCase) Then
				Return Double.NegativeInfinity
			Else
				Return Double.Parse(line)
			End If
		End Function

		Private Shared Function parseBoolean(ByVal line As String) As Boolean
			line = line.Trim()
			If line.matches("1(\.0*)?") Then 'Booleans are ocassionally represented like 1.000000 or 0.000000
				Return True
			ElseIf line.matches("0(\.0*)?") Then
				Return False
			End If
			Return Boolean.Parse(line)
		End Function


		Public Shared Function testPrecisionOverride(ByVal testName As String) As Pair(Of Double, Double)
			If "conv_4".Equals(testName, StringComparison.OrdinalIgnoreCase) Then
				'Most values: around 1k. So this is the 6th significant figure, which is OK
				Return New Pair(Of Double, Double)(1e-3, 1e-5)
			End If
			Return Nothing
		End Function

		Public Shared Function equalsWithEps(ByVal a As Double, ByVal b As Double) As Boolean
			Return Math.Abs(a - b) <= 0.00001
		End Function

		Public Shared Function getEqualityFunction(ByVal modelName As String, ByVal varName As String, ByVal tf As INDArray, ByVal sd As INDArray) As BiFunction(Of INDArray, INDArray, Boolean)
			If modelName.StartsWith("topk", StringComparison.Ordinal) Then
				Return Function(t, s) Nd4j.sort(t, True).Equals(Nd4j.sort(s, True))
			End If

			If modelName.StartsWith("empty", StringComparison.Ordinal) Then
				Return Function(t, s)
				Dim areEqualShapes As Boolean = t.equalShapes(s)
				Dim areEqualDataTypes As Boolean = t.dataType() = s.dataType()
				Return areEqualShapes AndAlso areEqualDataTypes
				End Function
			End If

			' sum of all elements along dimesions before and after shuffle has to be the same
			If modelName.StartsWith("random_shuffle", StringComparison.Ordinal) Then
				Return Function(t, s) Nd4j.sort(t, True).Equals(Nd4j.sort(s, True))
			End If

			If modelName.StartsWith("random_normal", StringComparison.Ordinal) Then
				Return Function(t, s)
				Dim areEqualShapes As Boolean = t.equalShapes(s)
				Dim meanS As Double = s.meanNumber().doubleValue()
				Dim meanT As Double = t.meanNumber().doubleValue()
				Dim stdS As Double = s.stdNumber().doubleValue()
				Dim stdT As Double = t.stdNumber().doubleValue()
				Dim eps As Double = 1
				Return areEqualShapes AndAlso (Math.Abs(meanS-meanT) < eps) AndAlso (Math.Abs(stdS-stdT) < eps)
				End Function
			End If

			If modelName.StartsWith("random_gamma", StringComparison.Ordinal) Then
				Return Function(t, s)
				Dim areEqualShapes As Boolean = t.equalShapes(s)
				Dim nonNegativeValues As Boolean = (t.minNumber().doubleValue() > 0) AndAlso (t.minNumber().doubleValue() > 0)
				Dim meanS As Double = s.meanNumber().doubleValue()
				Dim meanT As Double = t.meanNumber().doubleValue()
				Dim stdS As Double = s.stdNumber().doubleValue()
				Dim stdT As Double = t.stdNumber().doubleValue()
				Dim eps As Double = 1
				Return areEqualShapes AndAlso nonNegativeValues AndAlso (Math.Abs(meanS-meanT) < eps) AndAlso (Math.Abs(stdS-stdT) < eps)
				End Function
			End If

			If modelName.StartsWith("random_poisson", StringComparison.Ordinal) OrElse modelName.StartsWith("random_poisson_v2", StringComparison.Ordinal) Then
				Return Function(t, s)
				Dim areEqualShapes As Boolean = t.equalShapes(s)
				Dim nonNegativeValues As Boolean = (t.minNumber().doubleValue() >= 0) AndAlso (t.minNumber().doubleValue() >= 0)
				Dim meanS As Double = s.meanNumber().doubleValue()
				Dim meanT As Double = t.meanNumber().doubleValue()
				Dim stdS As Double = s.stdNumber().doubleValue()
				Dim stdT As Double = t.stdNumber().doubleValue()
				Dim eps As Double = 1
				Return areEqualShapes AndAlso nonNegativeValues AndAlso (Math.Abs(meanS-meanT) < eps) AndAlso (Math.Abs(stdS-stdT) < eps)
				End Function
			End If

			If modelName.StartsWith("random_uniform", StringComparison.Ordinal) OrElse modelName.StartsWith("random_uniform_int", StringComparison.Ordinal) Then
				Return Function(t, s)
				Dim areEqualShapes As Boolean = t.equalShapes(s)
				Dim meanS As Double = s.meanNumber().doubleValue()
				Dim meanT As Double = t.meanNumber().doubleValue()
				Dim stdS As Double = s.stdNumber().doubleValue()
				Dim stdT As Double = t.stdNumber().doubleValue()
				Dim eps As Double = 1
				Return areEqualShapes AndAlso (Math.Abs(stdS-stdT) < eps) AndAlso (Math.Abs(meanS-meanT) < eps)
				End Function
			End If

			If modelName.StartsWith("alpha_dropout", StringComparison.Ordinal) OrElse modelName.StartsWith("layers_dropout", StringComparison.Ordinal) OrElse modelName.StartsWith("dropout", StringComparison.Ordinal) Then
				'We can't compare dropout using simple equality due to randomness
				Return Function(t, s)
				Dim tfNums() As Double = t.ravel().toDoubleVector()
				Dim sdNums() As Double = s.ravel().toDoubleVector()

				Dim seen1 As Double? = Nothing, seen2 As Double? = Nothing
				For i As Integer = 0 To tfNums.Length - 1
					If Not equalsWithEps(tfNums(i), sdNums(i)) Then

						' if we have only seen one inequality so far, figure out which is the dropout
						If seen1 IsNot Nothing AndAlso seen2 IsNot Nothing Then
							If equalsWithEps(tfNums(i), seen1) OrElse equalsWithEps(sdNums(i), seen1) Then ' the dropout is in seen1
								seen2 = Nothing
							ElseIf equalsWithEps(tfNums(i), seen2) OrElse equalsWithEps(sdNums(i), seen2) Then ' the dropout is in seen2
								seen1 = seen2
								seen2 = Nothing
							Else ' neither match
								Return False
							End If
						End If

						If seen1 IsNot Nothing Then
							If Not equalsWithEps(tfNums(i), seen1) AndAlso Not equalsWithEps(sdNums(i), seen1) Then
								Return False
							End If
						Else
							seen1 = tfNums(i)
							seen2 = sdNums(i)
						End If
					End If
				Next i

				Return True
				End Function
			End If

			Return AddressOf Object.equals
		End Function

	End Class

End Namespace