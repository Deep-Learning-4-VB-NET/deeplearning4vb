Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports MultiDataSetWrapperIterator = org.deeplearning4j.datasets.iterator.MultiDataSetWrapperIterator
Imports CountingMultiDataSetIterator = org.deeplearning4j.integration.util.CountingMultiDataSetIterator
Imports Model = org.deeplearning4j.nn.api.Model
Imports BackpropType = org.deeplearning4j.nn.conf.BackpropType
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports InputPreProcessor = org.deeplearning4j.nn.conf.InputPreProcessor
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports Layer = org.deeplearning4j.nn.conf.layers.Layer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports GraphVertex = org.deeplearning4j.nn.graph.vertex.GraphVertex
Imports org.deeplearning4j.nn.layers
Imports FrozenLayer = org.deeplearning4j.nn.layers.FrozenLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports CollectScoresListener = org.deeplearning4j.optimize.listeners.CollectScoresListener
Imports ParallelInference = org.deeplearning4j.parallelism.ParallelInference
Imports InferenceMode = org.deeplearning4j.parallelism.inference.InferenceMode
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports History = org.nd4j.autodiff.listeners.records.History
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports org.nd4j.evaluation.classification
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Op = org.nd4j.linalg.api.ops.Op
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports RelativeError = org.nd4j.linalg.api.ops.impl.transforms.pairwise.RelativeError
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.nd4j.common.primitives
Imports Resources = org.nd4j.common.resources.Resources
Imports ImmutableSet = org.nd4j.shade.guava.collect.ImmutableSet
Imports ClassPath = org.nd4j.shade.guava.reflect.ClassPath
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j.integration



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class IntegrationTestRunner
	Public Class IntegrationTestRunner

		Public Const RANDOM_INIT_UNTRAINED_MODEL_FILENAME As String = "Model_RANDOM_INIT_UNTRAINED.zip"
		Public Const FLAT_GRADIENTS_FILENAME As String = "flattenedGradients.bin"
		Public Const TRAINING_CURVE_FILENAME As String = "trainingCurve.csv"
		Public Const PARAMS_POST_TRAIN_FILENAME As String = "paramsPostTrain.bin"
		Public Const PARAMS_POST_TRAIN_SAMEDIFF_DIR As String = "paramsPostTrain"
		Public Const PARAMS_POST_UNSUPERVISED_FILENAME As String = "paramsPostUnsupervised.bin"

		Public Const MAX_REL_ERROR_SCORES As Double = 1e-4

		Private Shared layerClasses As IList(Of Type) = New List(Of Type)()
		Private Shared preprocClasses As IList(Of Type) = New List(Of Type)()
		Private Shared graphVertexClasses As IList(Of Type) = New List(Of Type)()
		Private Shared evaluationClasses As IList(Of Type) = New List(Of Type)()

		Private Shared layerConfClassesSeen As IDictionary(Of Type, Integer) = New Dictionary(Of Type, Integer)()
		Private Shared preprocessorConfClassesSeen As IDictionary(Of Type, Integer) = New Dictionary(Of Type, Integer)()
		Private Shared vertexConfClassesSeen As IDictionary(Of Type, Integer) = New Dictionary(Of Type, Integer)()
		Private Shared evaluationClassesSeen As IDictionary(Of Type, Integer) = New Dictionary(Of Type, Integer)()

		Shared Sub New()
			Try
				setup()
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void setup() throws Exception
		Public Shared Sub setup()

			'First: discover all layers, preprocessors, etc

			Dim info As ImmutableSet(Of ClassPath.ClassInfo)
			Try
				'Dependency note: this ClassPath class was added in Guava 14
				info = ClassPath.from(GetType(DifferentialFunctionClassHolder).getClassLoader()).getTopLevelClassesRecursive("org.deeplearning4j")
			Catch e As IOException
				'Should never happen
				Throw New Exception(e)
			End Try

			For Each c As ClassPath.ClassInfo In info
				Dim clazz As Type = DL4JClassLoading.loadClassByName(c.getName())
				If Modifier.isAbstract(clazz.getModifiers()) OrElse clazz.IsInterface Then
					Continue For
				End If

				If isLayerConfig(clazz) Then
					layerClasses.Add(clazz)
				ElseIf isPreprocessorConfig(clazz) Then
					preprocClasses.Add(clazz)
				ElseIf isGraphVertexConfig(clazz) Then
					graphVertexClasses.Add(clazz)
				ElseIf isEvaluationClass(clazz) Then
					evaluationClasses.Add(clazz)
				End If
			Next c

			layerClasses.Sort(System.Collections.IComparer.comparing(AddressOf Type.getName))
			preprocClasses.Sort(System.Collections.IComparer.comparing(AddressOf Type.getName))
			graphVertexClasses.Sort(System.Collections.IComparer.comparing(AddressOf Type.getName))

			log.info("Found {} layers", layerClasses.Count)
			log.info("Found {} preprocessors", preprocClasses.Count)
			log.info("Found {} graph vertices", graphVertexClasses.Count)
			log.info("Found {} IEvaluation classes", evaluationClasses.Count)

			layerConfClassesSeen = New Dictionary(Of Type, Integer)()
			preprocessorConfClassesSeen = New Dictionary(Of Type, Integer)()
			vertexConfClassesSeen = New Dictionary(Of Type, Integer)()
			evaluationClassesSeen = New Dictionary(Of Type, Integer)()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void runTest(TestCase tc, java.nio.file.Path testDir) throws Exception
		Public Shared Sub runTest(ByVal tc As TestCase, ByVal testDir As Path)
			BaseDL4JTest.skipUnlessIntegrationTests() 'Tests will ONLY be run if integration test profile is enabled.
			'This could alternatively be done via maven surefire configuration

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ModelType modelType = tc.modelType();
			Dim modelType As ModelType = tc.modelType()
			log.info("Starting test case: {} - type = {}", tc.getTestName(), modelType)
			Dim start As Long = DateTimeHelper.CurrentUnixTimeMillis()

			Dim workingDir As New File(testDir.toFile(),"workingDir")
			tc.initialize(workingDir)

			Dim testBaseDir As New File(testDir.toFile(),"baseDir")
	'        new ClassPathResource("dl4j-integration-tests/" + tc.getTestName()).copyDirectory(testBaseDir);
			Resources.copyDirectory((If(modelType = ModelType.SAMEDIFF, "samediff-integration-tests/", "dl4j-integration-tests/")) + tc.getTestName(), testBaseDir)


			Dim mln As MultiLayerNetwork = Nothing
			Dim cg As ComputationGraph = Nothing
			Dim sd As SameDiff = Nothing
			Dim m As Model = Nothing
			If tc.getTestType() = TestCase.TestType.RANDOM_INIT Then
				log.info("Checking RANDOM_INIT test case: saved model vs. initialized model")
				'Checking randomly initialized model:
				Dim savedModel As New File(testBaseDir, IntegrationTestRunner.RANDOM_INIT_UNTRAINED_MODEL_FILENAME)
				Dim config As Object = tc.Configuration
				If TypeOf config Is MultiLayerConfiguration Then
					Dim mlc As MultiLayerConfiguration = DirectCast(config, MultiLayerConfiguration)
					mln = New MultiLayerNetwork(mlc)
					mln.init()
					m = mln

					Dim loaded As MultiLayerNetwork = MultiLayerNetwork.load(savedModel, True)
					assertEquals(loaded.LayerWiseConfigurations, mln.LayerWiseConfigurations, "Configs not equal")
					assertEquals(loaded.params(), mln.params(), "Params not equal")
					assertEquals(loaded.paramTable(), mln.paramTable(), "Param table not equal")
				ElseIf TypeOf config Is ComputationGraphConfiguration Then
					Dim cgc As ComputationGraphConfiguration = DirectCast(config, ComputationGraphConfiguration)
					cg = New ComputationGraph(cgc)
					cg.init()
					m = cg

					Dim loaded As ComputationGraph = ComputationGraph.load(savedModel, True)
					assertEquals(loaded.Configuration, cg.Configuration, "Configs not equal")
					assertEquals(loaded.params(), cg.params(), "Params not equal")
					assertEquals(loaded.paramTable(), cg.paramTable(), "Param table not equal")
				ElseIf TypeOf config Is SameDiff Then
					sd = DirectCast(config, SameDiff)
					Dim loaded As SameDiff = SameDiff.load(savedModel, True)

					assertSameDiffEquals(sd, loaded)
				Else
					Throw New System.InvalidOperationException("Unknown configuration/model type: " & config.GetType())
				End If
			Else
				m = tc.PretrainedModel
				If TypeOf m Is MultiLayerNetwork Then
					mln = DirectCast(m, MultiLayerNetwork)
				ElseIf TypeOf m Is ComputationGraph Then
					cg = DirectCast(m, ComputationGraph)
				ElseIf TypeOf m Is SameDiff Then
					sd = DirectCast(m, SameDiff)
				Else
					Throw New System.InvalidOperationException("Unknown model type: " & m.GetType())
				End If
			End If

			'Collect information for test coverage
			If modelType <> ModelType.SAMEDIFF Then
				collectCoverageInformation(m)
			End If


			'Check network output (predictions)
			If tc.isTestPredictions() Then
				log.info("Checking predictions: saved output vs. initialized model")


				Dim inputs As IList(Of Pair(Of INDArray(), INDArray())) = If(modelType <> ModelType.SAMEDIFF, tc.getPredictionsTestData(), Nothing)
				Dim inputsSd As IList(Of IDictionary(Of String, INDArray)) = If(modelType = ModelType.SAMEDIFF, tc.getPredictionsTestDataSameDiff(), Nothing)
				Preconditions.checkState(modelType = ModelType.SAMEDIFF OrElse inputs IsNot Nothing AndAlso inputs.Count > 0, "Input data is null or length 0 for test: %s", tc.getTestName())


				Dim predictionsTestDir As New File(testBaseDir, "predictions")
				predictionsTestDir.mkdirs()

				Dim count As Integer = 0
				If modelType = ModelType.MLN Then
					For Each p As Pair(Of INDArray(), INDArray()) In inputs
						Dim f As INDArray = p.First(0)
						Dim fm As INDArray = (If(p.Second Is Nothing, Nothing, p.Second(0)))
						Dim [out] As INDArray = mln.output(f, False, fm, Nothing)

						'Load the previously saved array
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: File outFile = new File(predictionsTestDir, "output_" + (count++) + "_0.bin");
						Dim outFile As New File(predictionsTestDir, "output_" & (count) & "_0.bin")
							count += 1
						Dim outSaved As INDArray
						Using dis As New DataInputStream(New FileStream(outFile, FileMode.Open, FileAccess.Read))
							outSaved = Nd4j.read(dis)
						End Using

						Dim predictionExceedsRE As INDArray = exceedsRelError(outSaved, [out], tc.getMaxRelativeErrorOutput(), tc.getMinAbsErrorOutput())
						Dim countExceeds As Integer = predictionExceedsRE.sumNumber().intValue()
						assertEquals(0, countExceeds,"Predictions do not match saved predictions - output")
					Next p
				ElseIf modelType = ModelType.CG Then
					For Each p As Pair(Of INDArray(), INDArray()) In inputs
						Dim [out]() As INDArray = cg.output(False, p.First, p.Second, Nothing)

						'Load the previously saved arrays
						Dim outSaved([out].Length - 1) As INDArray
						For i As Integer = 0 To [out].Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: File outFile = new File(predictionsTestDir, "output_" + (count++) + "_" + i + ".bin");
							Dim outFile As New File(predictionsTestDir, "output_" & (count) & "_" & i & ".bin")
								count += 1
							Using dis As New DataInputStream(New FileStream(outFile, FileMode.Open, FileAccess.Read))
								outSaved(i) = Nd4j.read(dis)
							End Using
						Next i

						For i As Integer = 0 To outSaved.Length - 1
							Dim predictionExceedsRE As INDArray = exceedsRelError(outSaved(i), [out](i), tc.getMaxRelativeErrorOutput(), tc.getMinAbsErrorOutput())
							Dim countExceeds As Integer = predictionExceedsRE.sumNumber().intValue()
							assertEquals(0, countExceeds,"Predictions do not match saved predictions - output " & i)
						Next i
					Next p
				Else
					Dim outNames As IList(Of String) = tc.getPredictionsNamesSameDiff()
					For Each ph As IDictionary(Of String, INDArray) In inputsSd
						Dim [out] As IDictionary(Of String, INDArray) = sd.output(ph, outNames)

						'Load the previously saved placeholder arrays
						Dim outSaved As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
						For Each s As String In outNames
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: File f = new File(predictionsTestDir, "output_" + (count++) + "_" + s + ".bin");
							Dim f As New File(predictionsTestDir, "output_" & (count) & "_" & s & ".bin")
								count += 1
							Using dis As New DataInputStream(New FileStream(f, FileMode.Open, FileAccess.Read))
								outSaved(s) = Nd4j.read(dis)
							End Using
						Next s

						For Each s As String In outNames
							Dim predictionExceedsRE As INDArray = exceedsRelError(outSaved(s), [out](s), tc.getMaxRelativeErrorOutput(), tc.getMinAbsErrorOutput())
							Dim countExceeds As Integer = predictionExceedsRE.sumNumber().intValue()
							assertEquals(0, countExceeds,"Predictions do not match saved predictions - output """ & s & """")
						Next s
					Next ph
				End If

				If modelType <> ModelType.SAMEDIFF Then
					checkLayerClearance(m)
				End If
			End If


			'Test gradients
			If tc.isTestGradients() Then
				log.info("Checking gradients: saved output vs. initialized model")

				Dim gradientFlat As INDArray = Nothing
				Dim layers() As org.deeplearning4j.nn.api.Layer = Nothing
				Dim grad As IDictionary(Of String, INDArray)
				If modelType = ModelType.MLN Then
					Dim data As MultiDataSet = tc.GradientsTestData
					mln.Input = data.getFeatures(0)
					mln.Labels = data.getLabels(0)
					mln.setLayerMaskArrays(data.getFeaturesMaskArray(0), data.getLabelsMaskArray(0))
					mln.computeGradientAndScore()
					gradientFlat = mln.getFlattenedGradients()
					layers = mln.Layers
					grad = mln.gradient().gradientForVariable()
				ElseIf modelType = ModelType.CG Then
					Dim data As MultiDataSet = tc.GradientsTestData
					cg.Inputs = data.Features
					cg.Labels = data.Labels
					cg.setLayerMaskArrays(data.FeaturesMaskArrays, data.LabelsMaskArrays)
					cg.computeGradientAndScore()
					gradientFlat = cg.getFlattenedGradients()
					layers = cg.Layers
					grad = cg.gradient().gradientForVariable()
				Else
					Dim ph As IDictionary(Of String, INDArray) = tc.getGradientsTestDataSameDiff()
					Dim allVars As IList(Of String) = New List(Of String)()
					For Each v As SDVariable In sd.variables()
						If v.getVariableType() = VariableType.VARIABLE Then
							allVars.Add(v.name())
						End If
					Next v
					grad = sd.calculateGradients(ph, allVars)
				End If

				If modelType <> ModelType.SAMEDIFF Then
					Dim gFlatFile As New File(testBaseDir, IntegrationTestRunner.FLAT_GRADIENTS_FILENAME)
					Dim gradientFlatSaved As INDArray = read(gFlatFile)

					Dim gradExceedsRE As INDArray = exceedsRelError(gradientFlatSaved, gradientFlat, tc.getMaxRelativeErrorGradients(), tc.getMinAbsErrorGradients())
					Dim count As Integer = gradExceedsRE.sumNumber().intValue()
					If count > 0 Then
						logFailedParams(20, "Gradient", layers, gradExceedsRE, gradientFlatSaved, gradientFlat)
					End If
					assertEquals(0, count,"Saved flattened gradients: not equal (using relative error)")
				End If

				'Load the gradient table:
				Dim gradientDir As New File(testBaseDir, "gradients")
				For Each f As File In gradientDir.listFiles()
					If Not f.isFile() Then
						Continue For
					End If
					Dim key As String = f.getName()
					key = key.Substring(0, key.Length - 4) 'remove ".bin"
					Dim loaded As INDArray = read(f)
					Dim now As INDArray = grad(key)


					Dim gradExceedsRE As INDArray = exceedsRelError(loaded, now, tc.getMaxRelativeErrorGradients(), tc.getMinAbsErrorGradients())
					Dim count As Integer = gradExceedsRE.sumNumber().intValue()
					assertEquals(0, count,"Gradients: not equal (using relative error) for parameter: " & key)
				Next f
			End If

			'Test layerwise pretraining
			If tc.isTestUnsupervisedTraining() Then
				log.info("Performing layerwise pretraining")
				Dim iter As MultiDataSetIterator = tc.UnsupervisedTrainData

				Dim paramsPostTraining As INDArray
				Dim layers() As org.deeplearning4j.nn.api.Layer
				If modelType = ModelType.MLN Then
					Dim layersToTrain() As Integer = tc.getUnsupervisedTrainLayersMLN()
					Preconditions.checkState(layersToTrain IsNot Nothing, "Layer indices must not be null")
					Dim dsi As DataSetIterator = New MultiDataSetWrapperIterator(iter)

					For Each i As Integer In layersToTrain
						mln.pretrainLayer(i, dsi)
					Next i
					paramsPostTraining = mln.params()
					layers = mln.Layers
				ElseIf modelType = ModelType.CG Then
					Dim layersToTrain() As String = tc.getUnsupervisedTrainLayersCG()
					Preconditions.checkState(layersToTrain IsNot Nothing, "Layer names must not be null")

					For Each i As String In layersToTrain
						cg.pretrainLayer(i, iter)
					Next i
					paramsPostTraining = cg.params()
					layers = cg.Layers
				Else
					Throw New System.NotSupportedException("Unsupported layerwise pretraining not supported for SameDiff models")
				End If

				Dim f As New File(testBaseDir, IntegrationTestRunner.PARAMS_POST_UNSUPERVISED_FILENAME)
				Dim expParams As INDArray = read(f)

				Dim exceedsRelError As INDArray = IntegrationTestRunner.exceedsRelError(expParams, paramsPostTraining, tc.getMaxRelativeErrorPretrainParams(), tc.getMinAbsErrorPretrainParams())
				Dim count As Integer = exceedsRelError.sumNumber().intValue()
				If count > 0 Then
					logFailedParams(20, "Parameter", layers, exceedsRelError, expParams, paramsPostTraining)
				End If
				assertEquals(0, count,"Number of parameters exceeding relative error")

				'Set params to saved ones - to avoid accumulation of roundoff errors causing later failures...
				m.Params = expParams
			End If


			'Test training curves:
			If tc.isTestTrainingCurves() OrElse tc.isTestParamsPostTraining() Then
				Dim trainData As MultiDataSetIterator = tc.TrainingData
				Dim isTbptt As Boolean
				Dim tbpttLength As Integer
				If modelType = ModelType.MLN Then
					isTbptt = mln.LayerWiseConfigurations.getBackpropType() = BackpropType.TruncatedBPTT
					tbpttLength = mln.LayerWiseConfigurations.getTbpttFwdLength()
				ElseIf modelType = ModelType.CG Then
					isTbptt = cg.Configuration.getBackpropType() = BackpropType.TruncatedBPTT
					tbpttLength = cg.Configuration.getTbpttFwdLength()
				Else
					isTbptt = False
					tbpttLength = 0
				End If

				Dim countingIter As New CountingMultiDataSetIterator(trainData, isTbptt, tbpttLength)
				Dim l As New CollectScoresListener(1)
				If modelType <> ModelType.SAMEDIFF Then
					m.setListeners(l)
				End If

				Dim iterBefore As Integer
				Dim epochBefore As Integer
				Dim iterAfter As Integer
				Dim epochAfter As Integer

				Dim frozenParamsBefore As IDictionary(Of String, INDArray) = If(modelType <> ModelType.SAMEDIFF, getFrozenLayerParamCopies(m), getConstantCopies(sd))
				Dim layers() As org.deeplearning4j.nn.api.Layer = Nothing
				Dim h As History = Nothing
				If modelType = ModelType.MLN Then
					iterBefore = mln.IterationCount
					epochBefore = mln.EpochCount
					mln.fit(countingIter)
					iterAfter = mln.IterationCount
					epochAfter = mln.EpochCount
					layers = mln.Layers
				ElseIf modelType = ModelType.CG Then
					iterBefore = cg.Configuration.getIterationCount()
					epochBefore = cg.Configuration.getEpochCount()
					cg.fit(countingIter)
					iterAfter = cg.Configuration.getIterationCount()
					epochAfter = cg.Configuration.getEpochCount()
					layers = cg.Layers
				Else
					iterBefore = sd.getTrainingConfig().getIterationCount()
					epochBefore = sd.getTrainingConfig().getEpochCount()
					h = sd.fit(countingIter, 1)
					iterAfter = sd.getTrainingConfig().getIterationCount()
					epochAfter = sd.getTrainingConfig().getEpochCount()
				End If

				'Check that frozen params (if any) haven't changed during training:
				If modelType = ModelType.SAMEDIFF Then
					checkConstants(frozenParamsBefore, sd)
				Else
					checkFrozenParams(frozenParamsBefore, m)
				End If

				'Validate the iteration and epoch counts - both for the net, and for the layers
				Dim newIters As Integer = countingIter.getCurrIter()
				assertEquals(iterBefore + newIters, iterAfter)
				assertEquals(epochBefore + 1, epochAfter)
				If modelType <> ModelType.SAMEDIFF Then
					validateLayerIterCounts(m, epochBefore + 1, iterBefore + newIters)
				End If


				Dim scores() As Double
				If modelType = ModelType.SAMEDIFF Then
					scores = h.lossCurve().getLossValues().toDoubleVector()
				Else
					scores = l.getListScore().toDoubleArray()
				End If

				Dim f As New File(testBaseDir, IntegrationTestRunner.TRAINING_CURVE_FILENAME)
				Dim s() As String = FileUtils.readFileToString(f, StandardCharsets.UTF_8).Split(",")

				If tc.isTestTrainingCurves() Then
					assertEquals(s.Length, scores.Length,"Different number of scores")

					Dim pass As Boolean = True
					For i As Integer = 0 To s.Length - 1
						Dim exp As Double = Double.Parse(s(i))
						Dim re As Double = relError(exp, scores(i))
						If re > MAX_REL_ERROR_SCORES Then
							pass = False
							Exit For
						End If
					Next i
					If Not pass Then
						fail("Scores differ: expected/saved: " & java.util.Arrays.toString(s) & vbLf & "Actual: " & java.util.Arrays.toString(scores))
					End If
				End If

				If tc.isTestParamsPostTraining() Then
					If modelType <> ModelType.SAMEDIFF Then
						Dim p As New File(testBaseDir, IntegrationTestRunner.PARAMS_POST_TRAIN_FILENAME)
						Dim paramsExp As INDArray = read(p)
						Dim z As INDArray = exceedsRelError(m.params(), paramsExp, tc.getMaxRelativeErrorParamsPostTraining(), tc.getMinAbsErrorParamsPostTraining())
						Dim count As Integer = z.sumNumber().intValue()
						If count > 0 Then
							logFailedParams(20, "Parameter", layers, z, paramsExp, m.params())
						End If
						assertEquals(0, count,"Number of params exceeded max relative error")
					Else
						Dim dir As New File(testBaseDir, IntegrationTestRunner.PARAMS_POST_TRAIN_SAMEDIFF_DIR)
						For Each v As SDVariable In sd.variables()
							If v.getVariableType() <> VariableType.VARIABLE Then
								Continue For
							End If
							Dim paramNow As INDArray = v.Arr
							Dim paramFile As New File(dir, v.name() & ".bin")
							Dim exp As INDArray = read(paramFile)
							Dim z As INDArray = exceedsRelError(paramNow, exp, tc.getMaxRelativeErrorParamsPostTraining(), tc.getMinAbsErrorParamsPostTraining())
							Dim count As Integer = z.sumNumber().intValue()
							If count > 0 Then
								logFailedParams(20, "Parameter: " & v.name(), layers, z, exp, paramNow)
							End If
							assertEquals(0, count,"Number of params exceeded max relative error for parameter: """ & v.name() & """")
						Next v
					End If
				End If

				If modelType <> ModelType.SAMEDIFF Then
					checkLayerClearance(m)
				End If
			End If

			'Check evaluation:
			If tc.isTestEvaluation() Then
				log.info("Testing evaluation")
				Dim evals() As IEvaluation = tc.NewEvaluations
				Dim iter As MultiDataSetIterator = tc.EvaluationTestData

				If modelType = ModelType.MLN Then
					Dim dsi As DataSetIterator = New MultiDataSetWrapperIterator(iter)
					mln.doEvaluation(dsi, evals)
				ElseIf modelType = ModelType.CG Then
					cg.doEvaluation(iter, evals)
				Else
					evals = tc.doEvaluationSameDiff(sd, iter, evals)
				End If

				Dim evalDir As New File(testBaseDir, "evaluation")
				For i As Integer = 0 To evals.Length - 1
					Dim f As New File(evalDir, i & "." & evals(i).GetType().Name & ".json")
					Dim json As String = FileUtils.readFileToString(f, StandardCharsets.UTF_8)
					Dim e As IEvaluation
					If evals(i).GetType() = GetType(Evaluation) Then
						e = Evaluation.fromJson(json)
					ElseIf evals(i).GetType() = GetType(RegressionEvaluation) Then
						e = RegressionEvaluation.fromJson(json, GetType(RegressionEvaluation))
					ElseIf evals(i).GetType() = GetType(ROC) Then
						e = ROC.fromJson(json, GetType(ROC))
					ElseIf evals(i).GetType() = GetType(ROCBinary) Then
						e = ROCBinary.fromJson(json, GetType(ROCBinary))
					ElseIf evals(i).GetType() = GetType(ROCMultiClass) Then
						e = ROCMultiClass.fromJson(json, GetType(ROCMultiClass))
					ElseIf evals(i).GetType() = GetType(EvaluationCalibration) Then
						e = EvaluationCalibration.fromJson(json, GetType(EvaluationCalibration))
					Else
						Throw New Exception("Unknown/not implemented evaluation type: " & evals(i).GetType())
					End If


					assertEquals(e, evals(i), "Evaluation not equal: " & evals(i).GetType())

					'Evaluation coverage information:
					evaluationClassesSeen(evals(i).GetType()) = evaluationClassesSeen.GetOrDefault(evals(i).GetType(), 0) + 1

					If modelType <> ModelType.SAMEDIFF Then
						checkLayerClearance(m)
					End If
				Next i
			End If

			'Check model serialization
			If True Then
				log.info("Testing model serialization")

				Dim f As New File(testDir.toFile(),"test-file")
				f.deleteOnExit()

				If modelType = ModelType.MLN Then
					ModelSerializer.writeModel(m, f, True)
					Dim restored As MultiLayerNetwork = MultiLayerNetwork.load(f, True)
					assertEquals(mln.LayerWiseConfigurations, restored.LayerWiseConfigurations)
					assertEquals(mln.params(), restored.params())
				ElseIf modelType = ModelType.CG Then
					ModelSerializer.writeModel(m, f, True)
					Dim restored As ComputationGraph = ComputationGraph.load(f, True)
					assertEquals(cg.Configuration, restored.Configuration)
					assertEquals(cg.params(), restored.params())
				Else
					sd.save(f, True)
					Dim restored As SameDiff = SameDiff.load(f, True)
					assertSameDiffEquals(sd, restored)
				End If

				System.GC.Collect()
			End If


			'Check parallel inference
			If modelType <> ModelType.SAMEDIFF AndAlso tc.isTestParallelInference() Then

				Dim inputs As IList(Of Pair(Of INDArray(), INDArray())) = tc.getPredictionsTestData()

				Dim numThreads As Integer = 2 'TODO allow customization of this?

				Dim exp As IList(Of INDArray()) = New List(Of INDArray())()
				For Each p As Pair(Of INDArray(), INDArray()) In inputs
					Dim [out]() As INDArray
					If modelType = ModelType.MLN Then
						Dim fm As INDArray = If(p.Second Is Nothing, Nothing, p.Second(0))
						[out] = New INDArray(){mln.output(p.First(0), False, fm, Nothing)}
					Else
						[out] = cg.output(False, p.First, p.Second, Nothing)
					End If
					exp.Add([out])
				Next p

				Dim inf As ParallelInference = (New ParallelInference.Builder(m)).inferenceMode(InferenceMode.BATCHED).batchLimit(3).queueLimit(8).workers(numThreads).build()


				testParallelInference(inf, inputs, exp)

				inf.shutdown()
				inf = Nothing
				System.GC.Collect()
			End If


			'Test overfitting single example
			If tc.isTestOverfitting() Then
				log.info("Testing overfitting on single example")

				Dim toOverfit As MultiDataSet = tc.OverfittingData
				Dim i As Integer = 0
				Do While i < tc.OverfitNumIterations
					If modelType = ModelType.MLN Then
						mln.fit(toOverfit)
					ElseIf modelType = ModelType.CG Then
						cg.fit(toOverfit)
					Else
						sd.fit(toOverfit)
					End If
					i += 1
				Loop

				'Check:
				Dim output() As INDArray = Nothing
				Dim outSd As IDictionary(Of String, INDArray) = Nothing
				If modelType = ModelType.MLN Then
					mln.setLayerMaskArrays(toOverfit.getFeaturesMaskArray(0), Nothing)
					output = New INDArray(){mln.output(toOverfit.getFeatures(0))}
				ElseIf modelType = ModelType.CG Then
					cg.setLayerMaskArrays(toOverfit.FeaturesMaskArrays, Nothing)
					output = cg.output(toOverfit.Features)
				Else
					Dim l As IList(Of String) = sd.getTrainingConfig().getDataSetFeatureMapping()
					Dim phMap As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
					Dim i As Integer=0
					For Each s As String In l
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: phMap.put(s, toOverfit.getFeatures(i++));
						phMap(s) = toOverfit.getFeatures(i)
							i += 1
					Next s
					outSd = sd.output(phMap, tc.getPredictionsNamesSameDiff())
				End If

				Dim n As Integer = If(modelType = ModelType.SAMEDIFF, outSd.Count, output.Length)
				For i As Integer = 0 To n - 1
					Dim [out] As INDArray = If(modelType = ModelType.SAMEDIFF, outSd(tc.getPredictionsNamesSameDiff()(i)), output(i))
					Dim label As INDArray = toOverfit.getLabels(i)

					Dim z As INDArray = exceedsRelError([out], label, tc.getMaxRelativeErrorOverfit(), tc.getMinAbsErrorOverfit())
					Dim count As Integer = z.sumNumber().intValue()
					If count > 0 Then
						Console.WriteLine([out])
						Console.WriteLine(label)
						Dim re As INDArray = relativeError([out], label, tc.getMinAbsErrorOverfit())
						Console.WriteLine("Relative error:")
						Console.WriteLine(re)
					End If
					assertEquals(0, count,"Number of outputs exceeded max relative error")
				Next i

				If modelType <> ModelType.SAMEDIFF Then
					checkLayerClearance(m)
				End If
			End If

			Dim [end] As Long = DateTimeHelper.CurrentUnixTimeMillis()


			log.info("Completed test case {} in {} sec", tc.getTestName(), ([end] - start) \ 1000L)
		End Sub

		'Work out which layers, vertices etc we have seen - so we can (at the end of all tests) log our integration test coverage
		Private Shared Sub collectCoverageInformation(ByVal m As Model)
			Dim isMLN As Boolean = (TypeOf m Is MultiLayerNetwork)
			Dim mln As MultiLayerNetwork = (If(isMLN, DirectCast(m, MultiLayerNetwork), Nothing))
			Dim cg As ComputationGraph = (If(Not isMLN, DirectCast(m, ComputationGraph), Nothing))

			'Collect layer coverage information:
			Dim layers() As org.deeplearning4j.nn.api.Layer
			If isMLN Then
				layers = mln.Layers
			Else
				layers = cg.Layers
			End If
			For Each l As org.deeplearning4j.nn.api.Layer In layers
				Dim lConf As Layer = l.conf().getLayer()
				layerConfClassesSeen(lConf.GetType()) = layerConfClassesSeen.GetOrDefault(lConf.GetType(), 0) + 1
			Next l

			'Collect preprocessor coverage information:
			Dim preProcessors As ICollection(Of InputPreProcessor)
			If isMLN Then
				preProcessors = mln.LayerWiseConfigurations.getInputPreProcessors().values()
			Else
				preProcessors = New List(Of InputPreProcessor)()
				For Each gv As org.deeplearning4j.nn.conf.graph.GraphVertex In cg.Configuration.getVertices().values()
					If TypeOf gv Is LayerVertex Then
						Dim pp As InputPreProcessor = DirectCast(gv, LayerVertex).PreProcessor
						If pp IsNot Nothing Then
							preProcessors.Add(pp)
						End If
					End If
				Next gv
			End If
			For Each ipp As InputPreProcessor In preProcessors
				preprocessorConfClassesSeen(ipp.GetType()) = preprocessorConfClassesSeen.GetOrDefault(ipp.GetType(), 0) + 1
			Next ipp

			'Collect vertex coverage information
			If Not isMLN Then
				For Each gv As org.deeplearning4j.nn.conf.graph.GraphVertex In cg.Configuration.getVertices().values()
					vertexConfClassesSeen(gv.GetType()) = vertexConfClassesSeen.GetOrDefault(gv.GetType(), 0) + 1
				Next gv
			End If
		End Sub


		Private Shared Sub checkLayerClearance(ByVal m As Model)
			'Check that the input fields for all layers have been cleared
			Dim layers() As org.deeplearning4j.nn.api.Layer
			If TypeOf m Is MultiLayerNetwork Then
				layers = DirectCast(m, MultiLayerNetwork).Layers
			Else
				layers = DirectCast(m, ComputationGraph).Layers
			End If

			For Each l As org.deeplearning4j.nn.api.Layer In layers
				assertNull(l.input())
				assertNull(l.MaskArray)
				If TypeOf l Is BaseOutputLayer Then
					Dim b As BaseOutputLayer = DirectCast(l, BaseOutputLayer)
					assertNull(b.getLabels())
				End If
			Next l


			If TypeOf m Is ComputationGraph Then
				'Also check the vertices:
				Dim vertices() As GraphVertex = DirectCast(m, ComputationGraph).Vertices
				For Each v As GraphVertex In vertices
					Dim numInputs As Integer = v.NumInputArrays
					Dim arr() As INDArray = v.Inputs
					If arr IsNot Nothing Then
						For i As Integer = 0 To numInputs - 1
							assertNull(arr(i))
						Next i
					End If
				Next v
			End If
		End Sub

		Private Shared Sub validateLayerIterCounts(ByVal m As Model, ByVal expEpoch As Integer, ByVal expIter As Integer)
			'Check that the iteration and epoch counts - on the layers - are synced
			Dim layers() As org.deeplearning4j.nn.api.Layer
			If TypeOf m Is MultiLayerNetwork Then
				layers = DirectCast(m, MultiLayerNetwork).Layers
			Else
				layers = DirectCast(m, ComputationGraph).Layers
			End If

			For Each l As org.deeplearning4j.nn.api.Layer In layers
				assertEquals(expEpoch, l.EpochCount,"Epoch count")
				assertEquals(expIter, l.IterationCount,"Iteration count")
			Next l
		End Sub


		Private Shared Function getFrozenLayerParamCopies(ByVal m As Model) As IDictionary(Of String, INDArray)
			Dim [out] As IDictionary(Of String, INDArray) = New LinkedHashMap(Of String, INDArray)()
			Dim layers() As org.deeplearning4j.nn.api.Layer
			If TypeOf m Is MultiLayerNetwork Then
				layers = DirectCast(m, MultiLayerNetwork).Layers
			Else
				layers = DirectCast(m, ComputationGraph).Layers
			End If

			For Each l As org.deeplearning4j.nn.api.Layer In layers
				If TypeOf l Is FrozenLayer Then
					Dim paramPrefix As String
					If TypeOf m Is MultiLayerNetwork Then
						paramPrefix = l.Index & "_"
					Else
						paramPrefix = l.conf().getLayer().getLayerName() & "_"
					End If
					Dim paramTable As IDictionary(Of String, INDArray) = l.paramTable()
					For Each e As KeyValuePair(Of String, INDArray) In paramTable.SetOfKeyValuePairs()
						[out](paramPrefix & e.Key) = e.Value.dup()
					Next e
				End If
			Next l

			Return [out]
		End Function

		Private Shared Function getConstantCopies(ByVal sd As SameDiff) As IDictionary(Of String, INDArray)
			Dim [out] As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			For Each v As SDVariable In sd.variables()
				If v.Constant Then
					[out](v.name()) = v.Arr
				End If
			Next v
			Return [out]
		End Function

		Public Shared Sub checkFrozenParams(ByVal copiesBeforeTraining As IDictionary(Of String, INDArray), ByVal m As Model)
			For Each e As KeyValuePair(Of String, INDArray) In copiesBeforeTraining.SetOfKeyValuePairs()
				Dim actual As INDArray = m.getParam(e.Key)
				assertEquals(e.Value, actual, e.Key)
			Next e
		End Sub

		Public Shared Sub checkConstants(ByVal copiesBefore As IDictionary(Of String, INDArray), ByVal sd As SameDiff)
			For Each e As KeyValuePair(Of String, INDArray) In copiesBefore.SetOfKeyValuePairs()
				Dim actual As INDArray = sd.getArrForVarName(e.Key)
				assertEquals(e.Value, actual, e.Key)
			Next e
		End Sub

		Public Shared Sub printCoverageInformation()

			log.info("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||")

			log.info("Layer coverage - classes seen:")
			For Each c As Type In layerClasses
				If layerConfClassesSeen.ContainsKey(c) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("Class seen {} times in tests: {}", layerConfClassesSeen(c), c.FullName)
				End If
			Next c

			log.info("Layer classes NOT seen in any tests:")
			For Each c As Type In layerClasses
				If Not layerConfClassesSeen.ContainsKey(c) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("Class NOT seen in any tests: {}", c.FullName)
				End If
			Next c

			log.info("----------------------------------------------------------------------------------------------------")

			log.info("GraphVertex coverage - classes seen:")
			For Each c As Type In graphVertexClasses
				If vertexConfClassesSeen.ContainsKey(c) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("Preprocessor seen {} times in tests: {}", preprocessorConfClassesSeen(c), c.FullName)
				End If
			Next c

			log.info("GraphVertexcoverage - classes NOT seen:")
			For Each c As Type In graphVertexClasses
				If Not vertexConfClassesSeen.ContainsKey(c) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("Preprocessor NOT seen in any tests: {}", c.FullName)
				End If
			Next c

			log.info("----------------------------------------------------------------------------------------------------")

			log.info("Preprocessor coverage - classes seen:")
			For Each c As Type In preprocClasses
				If preprocessorConfClassesSeen.ContainsKey(c) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("Preprocessor seen {} times in tests: {}", preprocessorConfClassesSeen(c), c.FullName)
				End If
			Next c

			log.info("Preprocessor coverage - classes NOT seen:")
			For Each c As Type In preprocClasses
				If Not preprocessorConfClassesSeen.ContainsKey(c) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("Preprocessor NOT seen in any tests: {}", c.FullName)
				End If
			Next c

			log.info("----------------------------------------------------------------------------------------------------")


			log.info("Evaluation coverage - classes seen:")
			For Each c As Type In evaluationClasses
				If evaluationClassesSeen.ContainsKey(c) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("Evaluation class seen {} times in tests: {}", evaluationClassesSeen(c), c.FullName)
				End If
			Next c

			log.info("Evaluation coverage - classes NOT seen:")
			For Each c As Type In evaluationClasses
				If Not evaluationClassesSeen.ContainsKey(c) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.info("Evaluation class NOT seen in any tests: {}", c.FullName)
				End If
			Next c

			log.info("----------------------------------------------------------------------------------------------------")
		End Sub

		Private Shared Function isLayerConfig(ByVal c As Type) As Boolean
			Return c.IsAssignableFrom(GetType(Layer))
		End Function

		Private Shared Function isPreprocessorConfig(ByVal c As Type) As Boolean
			Return c.IsAssignableFrom(GetType(InputPreProcessor))
		End Function

		Private Shared Function isGraphVertexConfig(ByVal c As Type) As Boolean
			Return c.IsAssignableFrom(GetType(GraphVertex))
		End Function

		Private Shared Function isEvaluationClass(ByVal c As Type) As Boolean
			Return c.IsAssignableFrom(GetType(IEvaluation))
		End Function

		Private Shared Function read(ByVal f As File) As INDArray
			Try
					Using dis As New DataInputStream(New BufferedInputStream(New FileStream(f, FileMode.Open, FileAccess.Read)))
					Return Nd4j.read(dis)
					End Using
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Sub write(ByVal arr As INDArray, ByVal f As File)
			Try
					Using dos As New DataOutputStream(New BufferedOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write)))
					Nd4j.write(arr, dos)
					End Using
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Sub

		Private Shared Function relError(ByVal d1 As Double, ByVal d2 As Double) As Double
			Preconditions.checkState(Not Double.IsNaN(d1), "d1 is NaN")
			Preconditions.checkState(Not Double.IsNaN(d2), "d2 is NaN")
			If d1 = 0.0 AndAlso d2 = 0.0 Then
				Return 0.0
			End If

			Return Math.Abs(d1 - d2) / (Math.Abs(d1) + Math.Abs(d2))
		End Function

		Private Shared Function exceedsRelError(ByVal first As INDArray, ByVal second As INDArray, ByVal maxRel As Double, ByVal minAbs As Double) As INDArray
	'        INDArray z = Nd4j.createUninitialized(first.shape());
	'        Op op = new BinaryMinimalRelativeError(first, second, z, maxRel, minAbs);
	'        Nd4j.getExecutioner().exec(op);
	'        return z;
			Dim z As INDArray = relativeError(first, second, minAbs)
			BooleanIndexing.replaceWhere(z, 0.0, Conditions.lessThan(maxRel))
			BooleanIndexing.replaceWhere(z, 1.0, Conditions.greaterThan(0.0))
			Return z
		End Function

		Private Shared Function relativeError(ByVal first As INDArray, ByVal second As INDArray) As INDArray
			Dim z As INDArray = Nd4j.createUninitialized(first.shape())
			Dim op As Op = New RelativeError(first, second, z)
			Nd4j.Executioner.exec(op)
			Return z
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private static org.nd4j.linalg.api.ndarray.INDArray relativeError(@NonNull INDArray a1, @NonNull INDArray a2, double minAbsError)
		Private Shared Function relativeError(ByVal a1 As INDArray, ByVal a2 As INDArray, ByVal minAbsError As Double) As INDArray
			Dim numNaN1 As Long = Nd4j.Executioner.exec(New MatchCondition(a1, Conditions.Nan, Integer.MaxValue)).getInt(0)
			Dim numNaN2 As Long = Nd4j.Executioner.exec(New MatchCondition(a2, Conditions.Nan, Integer.MaxValue)).getInt(0)
			Preconditions.checkState(numNaN1 = 0, "Array 1 has NaNs")
			Preconditions.checkState(numNaN2 = 0, "Array 2 has NaNs")


	'        INDArray isZero1 = a1.eq(0.0);
	'        INDArray isZero2 = a2.eq(0.0);
	'        INDArray bothZero = isZero1.muli(isZero2);

			Dim abs1 As INDArray = Transforms.abs(a1, True)
			Dim abs2 As INDArray = Transforms.abs(a2, True)
			Dim absDiff As INDArray = Transforms.abs(a1.sub(a2), False)

			'abs(a1-a2) < minAbsError ? 1 : 0
			Dim greaterThanMinAbs As INDArray = Transforms.abs(a1.sub(a2), False)
			BooleanIndexing.replaceWhere(greaterThanMinAbs, 0.0, Conditions.lessThan(minAbsError))
			BooleanIndexing.replaceWhere(greaterThanMinAbs, 1.0, Conditions.greaterThan(0.0))

			Dim result As INDArray = absDiff.divi(abs1.add(abs2))
			'Only way to have NaNs given there weren't any in original : both 0s
			BooleanIndexing.replaceWhere(result, 0.0, Conditions.Nan)
			'Finally, set to 0 if less than min abs error, or unchanged otherwise
			result.muli(greaterThanMinAbs)

	'        double maxRE = result.maxNumber().doubleValue();
	'        if(maxRE > MAX_REL_ERROR){
	'            System.out.println();
	'        }
			Return result
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void testParallelInference(@NonNull ParallelInference inf, List<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray[],org.nd4j.linalg.api.ndarray.INDArray[]>> in, List<org.nd4j.linalg.api.ndarray.INDArray[]> exp) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub testParallelInference(ByVal inf As ParallelInference, ByVal [in] As IList(Of Pair(Of INDArray(), INDArray())), ByVal exp As IList(Of INDArray()))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray[][] act = new org.nd4j.linalg.api.ndarray.INDArray[in.size()][0];
'JAVA TO VB CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
'ORIGINAL LINE: Dim act[][] As INDArray = new INDArray[in.Count][0]
			Dim act()() As INDArray = RectangularArrays.RectangularINDArrayArray([in].Count, 0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger counter = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim counter As New AtomicInteger(0)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.concurrent.atomic.AtomicInteger failedCount = new java.util.concurrent.atomic.AtomicInteger(0);
			Dim failedCount As New AtomicInteger(0)

			For i As Integer = 0 To [in].Count - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int j=i;
				Dim j As Integer=i
				Call (New Thread(Sub()
				Try
					Dim inMask() As INDArray = [in](j).getSecond()
					act(j) = inf.output([in](j).getFirst(), inMask)
					counter.incrementAndGet()
				Catch e As Exception
					log.error("",e)
					failedCount.incrementAndGet()
				End Try
				End Sub)).Start()
			Next i

			Dim start As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Dim current As Long = DateTimeHelper.CurrentUnixTimeMillis()
			Do While current < start + 20000 AndAlso failedCount.get() = 0 AndAlso counter.get() < [in].Count
				Thread.Sleep(1000L)
			Loop

			assertEquals(0, failedCount.get())
			assertEquals([in].Count, counter.get())
			For i As Integer = 0 To [in].Count - 1
				Dim e() As INDArray = exp(i)
				Dim a() As INDArray = act(i)

				assertArrayEquals(e, a)
			Next i
		End Sub


		Public Shared Sub logFailedParams(ByVal maxNumToPrintOnFailure As Integer, ByVal prefix As String, ByVal layers() As org.deeplearning4j.nn.api.Layer, ByVal exceedsRelError As INDArray, ByVal exp As INDArray, ByVal act As INDArray)
			Dim length As Long = exceedsRelError.length()
			Dim logCount As Integer = 0
			For i As Integer = 0 To length - 1
				If exceedsRelError.getDouble(i) > 0 Then
					Dim dExp As Double = exp.getDouble(i)
					Dim dAct As Double = act.getDouble(i)
					Dim re As Double = relError(dExp, dAct)
					Dim ae As Double = Math.Abs(dExp - dAct)

					'Work out parameter key:
					Dim pSoFar As Long = 0
					Dim pName As String = Nothing
					For Each l As org.deeplearning4j.nn.api.Layer In layers
						Dim n As Long = l.numParams()
						If pSoFar + n < i Then
							pSoFar += n
						Else
							For Each e As KeyValuePair(Of String, INDArray) In l.paramTable().SetOfKeyValuePairs()
								pSoFar += e.Value.length()
								If pSoFar >= i Then
									pName = e.Key
									Exit For
								End If
							Next e
						End If
					Next l

					log.info("{} {} ({}) failed: expected {} vs actual {} (RelativeError: {}, AbsError: {})", i, prefix, pName, dExp, dAct, re, ae)
					logCount += 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: if(++logCount >= maxNumToPrintOnFailure)
					If logCount >= maxNumToPrintOnFailure Then
						Exit For
					End If
				End If
			Next i
		End Sub

		Public Shared Sub assertSameDiffEquals(ByVal sd1 As SameDiff, ByVal sd2 As SameDiff)
			assertEquals(sd1.variableMap().Keys, sd2.variableMap().Keys)
			assertEquals(sd1.getOps().keySet(), sd2.getOps().keySet())
			assertEquals(sd1.inputs(), sd2.inputs())

			'Check constant and variable arrays:
			For Each v As SDVariable In sd1.variables()
				Dim n As String = v.name()
				assertEquals(v.getVariableType(), sd2.getVariable(n).getVariableType(), n)
				If v.Constant OrElse v.getVariableType() = VariableType.VARIABLE Then
					Dim a1 As INDArray = v.Arr
					Dim a2 As INDArray = sd2.getVariable(n).Arr
					assertEquals(a1, a2, n)
				End If
			Next v

			'Check ops:
			For Each o As SameDiffOp In sd1.getOps().values()
				Dim o2 As SameDiffOp = sd2.getOps().get(o.Name)
				assertEquals(o.Op.GetType(), o2.Op.GetType())
			Next o
		End Sub
	End Class

End Namespace