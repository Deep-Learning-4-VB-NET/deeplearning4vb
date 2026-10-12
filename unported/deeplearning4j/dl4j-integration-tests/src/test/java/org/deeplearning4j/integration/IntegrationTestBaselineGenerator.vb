Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileUtils = org.apache.commons.io.FileUtils
Imports MultiDataSetWrapperIterator = org.deeplearning4j.datasets.iterator.MultiDataSetWrapperIterator
Imports SameDiffCNNCases = org.deeplearning4j.integration.testcases.samediff.SameDiffCNNCases
Imports SameDiffMLPTestCases = org.deeplearning4j.integration.testcases.samediff.SameDiffMLPTestCases
Imports SameDiffRNNTestCases = org.deeplearning4j.integration.testcases.samediff.SameDiffRNNTestCases
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports CollectScoresListener = org.deeplearning4j.optimize.listeners.CollectScoresListener
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports History = org.nd4j.autodiff.listeners.records.History
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports org.nd4j.evaluation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.primitives
Imports Files = org.nd4j.shade.guava.io.Files
import static org.junit.jupiter.api.Assertions.assertEquals

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
'ORIGINAL LINE: @Slf4j public class IntegrationTestBaselineGenerator
	Public Class IntegrationTestBaselineGenerator

		Public Shared ReadOnly OUTPUT_DIR_DL4J As File = (New File("../../dl4j-test-resources/src/main/resources/dl4j-integration-tests")).getAbsoluteFile()
		Public Shared ReadOnly OUTPUT_DIR_SAMEDIFF As File = (New File("../../dl4j-test-resources/src/main/resources/samediff-integration-tests")).getAbsoluteFile()


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void main(String[] args) throws Exception
		Public Shared Sub Main(ByVal args() As String)
			If Not OUTPUT_DIR_DL4J.exists() AndAlso Not OUTPUT_DIR_SAMEDIFF.exists() Then
				Throw New Exception("output directories in test resources do not exist!")
			End If

			runGeneration(SameDiffMLPTestCases.MLPMnist, SameDiffMLPTestCases.MLPMoon, SameDiffCNNCases.LenetMnist, SameDiffCNNCases.Cnn3dSynthetic, SameDiffRNNTestCases.RnnCsvSequenceClassificationTestCase1)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static void runGeneration(TestCase... testCases) throws Exception
		Private Shared Sub runGeneration(ParamArray ByVal testCases() As TestCase)

			For Each tc As TestCase In testCases
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ModelType modelType = tc.modelType();
				Dim modelType As ModelType = tc.modelType()

				'Basic validation:
				Preconditions.checkState(tc.getTestName() IsNot Nothing, "Test case name is null")

				'Run through each test case:
				Dim testBaseDir As New File(If(modelType = ModelType.SAMEDIFF, OUTPUT_DIR_SAMEDIFF, OUTPUT_DIR_DL4J), tc.getTestName())
				If testBaseDir.exists() Then
					FileUtils.forceDelete(testBaseDir)
				End If
				testBaseDir.mkdirs()

				Dim workingDir As File = Files.createTempDir()
				tc.initialize(workingDir)

				log.info("Starting result generation for test ""{}"" - output directory: {}", tc.getTestName(), testBaseDir.getAbsolutePath())

				'Step 0: collect metadata for the current machine, and write it (in case we need to debug anything related to
				' the comparison data)
				Dim properties As Properties = Nd4j.Executioner.EnvironmentInformation
				Dim pCopy As New Properties()
				Dim comment As String = System.getProperty("user.name") & " - " & DateTimeHelper.CurrentUnixTimeMillis()
	'        StringBuilder sb = new StringBuilder(comment).append("\n");
				Using os As Stream = New BufferedOutputStream(New FileStream(testBaseDir, "nd4jEnvironmentInfo.json", FileMode.Create, FileAccess.Write))
					Dim e As IEnumerator(Of Object) = properties.keys()
					Do While e.MoveNext()
						Dim k As Object = e.Current
						Dim v As Object = properties.get(k)
						pCopy.setProperty(k.ToString(),If(v Is Nothing, "null", v.ToString()))
					Loop
					pCopy.store(os, comment)
				End Using


				'First: if test is a random init test: generate the config, and save it
				Dim mln As MultiLayerNetwork = Nothing
				Dim cg As ComputationGraph = Nothing
				Dim sd As SameDiff = Nothing
				Dim m As Model = Nothing
				If tc.getTestType() = TestCase.TestType.RANDOM_INIT Then
					Dim config As Object = tc.Configuration
					Dim json As String = Nothing
					If TypeOf config Is MultiLayerConfiguration Then
						Dim mlc As MultiLayerConfiguration = DirectCast(config, MultiLayerConfiguration)
						json = mlc.toJson()
						mln = New MultiLayerNetwork(mlc)
						mln.init()
						m = mln
					ElseIf TypeOf config Is ComputationGraphConfiguration Then
						Dim cgc As ComputationGraphConfiguration = DirectCast(config, ComputationGraphConfiguration)
						json = cgc.toJson()
						cg = New ComputationGraph(cgc)
						cg.init()
						m = cg
					Else
						sd = DirectCast(config, SameDiff)
					End If

					Dim savedModel As New File(testBaseDir, IntegrationTestRunner.RANDOM_INIT_UNTRAINED_MODEL_FILENAME)
					If modelType <> ModelType.SAMEDIFF Then
						Dim configFile As New File(testBaseDir, "config." & (If(modelType = ModelType.MLN, "mlc.json", "cgc.json")))
						FileUtils.writeStringToFile(configFile, json, StandardCharsets.UTF_8)
						log.info("RANDOM_INIT test - saved configuration: {}", configFile.getAbsolutePath())
						ModelSerializer.writeModel(m, savedModel, True)
					Else
						sd.save(savedModel, True)
					End If
					log.info("RANDOM_INIT test - saved randomly initialized model to: {}", savedModel.getAbsolutePath())
				Else
					'Pretrained model
					m = tc.PretrainedModel
					If TypeOf m Is MultiLayerNetwork Then
						mln = DirectCast(m, MultiLayerNetwork)
					ElseIf TypeOf m Is ComputationGraph Then
						cg = DirectCast(m, ComputationGraph)
					Else
						sd = DirectCast(m, SameDiff)
					End If
				End If


				'Generate predictions to compare against
				If tc.isTestPredictions() Then
					Dim inputs As IList(Of Pair(Of INDArray(), INDArray())) = If(modelType <> ModelType.SAMEDIFF, tc.getPredictionsTestData(), Nothing)
					Dim inputsSd As IList(Of IDictionary(Of String, INDArray)) = If(modelType = ModelType.SAMEDIFF, tc.getPredictionsTestDataSameDiff(), Nothing)
	'                Preconditions.checkState(inputs != null && inputs.size() > 0, "Input data is null or length 0 for test: %s", tc.getTestName());


					Dim predictionsTestDir As New File(testBaseDir, "predictions")
					predictionsTestDir.mkdirs()

					Dim count As Integer = 0
					If modelType = ModelType.MLN Then
						For Each p As Pair(Of INDArray(), INDArray()) In inputs
							Dim f As INDArray = p.First(0)
							Dim fm As INDArray = (If(p.Second Is Nothing, Nothing, p.Second(0)))
							Dim [out] As INDArray = mln.output(f, False, fm, Nothing)

							'Save the array...
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: File outFile = new File(predictionsTestDir, "output_" + (count++) + "_0.bin");
							Dim outFile As New File(predictionsTestDir, "output_" & (count) & "_0.bin")
								count += 1
							Using dos As New DataOutputStream(New FileStream(outFile, FileMode.Create, FileAccess.Write))
								Nd4j.write([out], dos)
							End Using
						Next p
					ElseIf modelType = ModelType.CG Then
						For Each p As Pair(Of INDArray(), INDArray()) In inputs
							Dim [out]() As INDArray = cg.output(False, p.First, p.Second, Nothing)

							'Save the array(s)...
							For i As Integer = 0 To [out].Length - 1
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: File outFile = new File(predictionsTestDir, "output_" + (count++) + "_" + i + ".bin");
								Dim outFile As New File(predictionsTestDir, "output_" & (count) & "_" & i & ".bin")
									count += 1
								Using dos As New DataOutputStream(New FileStream(outFile, FileMode.Create, FileAccess.Write))
									Nd4j.write([out](i), dos)
								End Using
							Next i
						Next p
					Else
						Dim outNames As IList(Of String) = tc.getPredictionsNamesSameDiff()
						For Each ph As IDictionary(Of String, INDArray) In inputsSd
							Dim [out] As IDictionary(Of String, INDArray) = sd.output(ph, outNames)

							'Save the output...
							For Each s As String In outNames
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: File f = new File(predictionsTestDir, "output_" + (count++) + "_" + s + ".bin");
								Dim f As New File(predictionsTestDir, "output_" & (count) & "_" & s & ".bin")
									count += 1
								Using dos As New DataOutputStream(New FileStream(f, FileMode.Create, FileAccess.Write))
									Nd4j.write([out](s), dos)
								End Using
							Next s
						Next ph
					End If

					log.info("Saved predictions for {} inputs to disk in directory: {}", tc.getTestName(), predictionsTestDir)
				End If

				'Compute and save gradients:
				If tc.isTestGradients() Then
					Dim gradientFlat As INDArray = Nothing
					Dim grad As IDictionary(Of String, INDArray)
					If modelType = ModelType.MLN Then
						Dim data As MultiDataSet = tc.GradientsTestData
						mln.Input = data.getFeatures(0)
						mln.Labels = data.getLabels(0)
						mln.setLayerMaskArrays(data.getFeaturesMaskArray(0), data.getLabelsMaskArray(0))
						mln.computeGradientAndScore()
						gradientFlat = mln.getFlattenedGradients()
						grad = m.gradient().gradientForVariable()
					ElseIf modelType = ModelType.CG Then
						Dim data As MultiDataSet = tc.GradientsTestData
						cg.Inputs = data.Features
						cg.Labels = data.Labels
						cg.setLayerMaskArrays(data.FeaturesMaskArrays, data.LabelsMaskArrays)
						cg.computeGradientAndScore()
						gradientFlat = cg.getFlattenedGradients()
						grad = m.gradient().gradientForVariable()
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
						IntegrationTestRunner.write(gradientFlat, gFlatFile)
					End If

					'Also save the gradient param table:
					Dim gradientDir As New File(testBaseDir, "gradients")
					gradientDir.mkdir()
					For Each s As String In grad.Keys
						Dim f As New File(gradientDir, s & ".bin")
						IntegrationTestRunner.write(grad(s), f)
					Next s
				End If

				'Test pretraining
				If tc.isTestUnsupervisedTraining() Then
					log.info("Performing layerwise pretraining")
					Dim iter As MultiDataSetIterator = tc.UnsupervisedTrainData

					Dim paramsPostTraining As INDArray
					If modelType = ModelType.MLN Then
						Dim layersToTrain() As Integer = tc.getUnsupervisedTrainLayersMLN()
						Preconditions.checkState(layersToTrain IsNot Nothing, "Layer indices must not be null")
						Dim dsi As DataSetIterator = New MultiDataSetWrapperIterator(iter)

						For Each i As Integer In layersToTrain
							mln.pretrainLayer(i, dsi)
						Next i
						paramsPostTraining = mln.params()
					ElseIf modelType = ModelType.CG Then
						Dim layersToTrain() As String = tc.getUnsupervisedTrainLayersCG()
						Preconditions.checkState(layersToTrain IsNot Nothing, "Layer names must not be null")

						For Each i As String In layersToTrain
							cg.pretrainLayer(i, iter)
						Next i
						paramsPostTraining = cg.params()
					Else
						Throw New System.NotSupportedException("SameDiff not supported for unsupervised training tests")
					End If

					'Save params
					Dim f As New File(testBaseDir, IntegrationTestRunner.PARAMS_POST_UNSUPERVISED_FILENAME)
					IntegrationTestRunner.write(paramsPostTraining, f)
				End If

				'Test training curves:
				If tc.isTestTrainingCurves() Then
					Dim trainData As MultiDataSetIterator = tc.TrainingData

					Dim l As New CollectScoresListener(1)
					If modelType <> ModelType.SAMEDIFF Then
						m.setListeners(l)
					End If

					Dim h As History = Nothing
					If modelType = ModelType.MLN Then
						mln.fit(trainData)
					ElseIf modelType = ModelType.CG Then
						cg.fit(trainData)
					Else
						h = sd.fit(trainData, 1)
					End If

					Dim scores() As Double
					If modelType <> ModelType.SAMEDIFF Then
						scores = l.getListScore().toDoubleArray()
					Else
						scores = h.lossCurve().getLossValues().toDoubleVector()
					End If

					Dim f As New File(testBaseDir, IntegrationTestRunner.TRAINING_CURVE_FILENAME)
					Dim s As IList(Of String) = java.util.Arrays.stream(scores).mapToObj(AddressOf String.valueOf).collect(Collectors.toList())
					FileUtils.writeStringToFile(f, String.join(",", s), StandardCharsets.UTF_8)

					If tc.isTestParamsPostTraining() Then
						If modelType = ModelType.SAMEDIFF Then
							Dim p As New File(testBaseDir, IntegrationTestRunner.PARAMS_POST_TRAIN_SAMEDIFF_DIR)
							p.mkdirs()
							For Each v As SDVariable In sd.variables()
								If v.getVariableType() = VariableType.VARIABLE Then
									Dim arr As INDArray = v.Arr
									Dim p2 As New File(p, v.name() & ".bin")
									IntegrationTestRunner.write(arr, p2)
								End If
							Next v
						Else
							Dim p As New File(testBaseDir, IntegrationTestRunner.PARAMS_POST_TRAIN_FILENAME)
							IntegrationTestRunner.write(m.params(), p)
						End If
					End If
				End If

				If tc.isTestEvaluation() Then
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
					evalDir.mkdir()
					For i As Integer = 0 To evals.Length - 1
						Dim json As String = evals(i).toJson()
						Dim f As New File(evalDir, i & "." & evals(i).GetType().Name & ".json")
						FileUtils.writeStringToFile(f, json, StandardCharsets.UTF_8)
					Next i
				End If

				'Don't need to do anything here re: overfitting
			Next tc

			log.info("----- Completed test result generation -----")
		End Sub
	End Class

End Namespace