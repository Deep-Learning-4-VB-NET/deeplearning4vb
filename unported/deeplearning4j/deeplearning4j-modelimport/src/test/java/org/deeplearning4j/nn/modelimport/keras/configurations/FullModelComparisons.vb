Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports SequenceRecordReader = org.datavec.api.records.reader.SequenceRecordReader
Imports CSVSequenceRecordReader = org.datavec.api.records.reader.impl.csv.CSVSequenceRecordReader
Imports NumberedFileInputSplit = org.datavec.api.split.NumberedFileInputSplit
Imports SequenceRecordReaderDataSetIterator = org.deeplearning4j.datasets.datavec.SequenceRecordReaderDataSetIterator
Imports LSTM = org.deeplearning4j.nn.layers.recurrent.LSTM
Imports LastTimeStepLayer = org.deeplearning4j.nn.layers.recurrent.LastTimeStepLayer
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports KerasModel = org.deeplearning4j.nn.modelimport.keras.KerasModel
Imports KerasSequentialModel = org.deeplearning4j.nn.modelimport.keras.KerasSequentialModel
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports UnsupportedKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.UnsupportedKerasConfigurationException
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports ActivationHardSigmoid = org.nd4j.linalg.activations.impl.ActivationHardSigmoid
Imports ActivationTanH = org.nd4j.linalg.activations.impl.ActivationTanH
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports Resources = org.nd4j.common.resources.Resources
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.nn.modelimport.keras.configurations


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class FullModelComparisons extends org.deeplearning4j.BaseDL4JTest
	Public Class FullModelComparisons
		Inherits BaseDL4JTest

		Friend classLoader As ClassLoader = GetType(FullModelComparisons).getClassLoader()


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void lstmTest(@TempDir Path testDir) throws IOException, UnsupportedKerasConfigurationException, InvalidKerasConfigurationException, InterruptedException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub lstmTest(ByVal testDir As Path)

			Dim modelPath As String = "modelimport/keras/fullconfigs/lstm/lstm_th_keras_2_config.json"
			Dim weightsPath As String = "modelimport/keras/fullconfigs/lstm/lstm_th_keras_2_weights.h5"

			Dim kerasModel As KerasSequentialModel
			Using modelIS As Stream = org.nd4j.common.resources.Resources.asStream(modelPath)

				 kerasModel = (New KerasModel()).modelBuilder().modelJsonInputStream(modelIS).weightsHdf5FilenameNoRoot(Resources.asFile(weightsPath).getAbsolutePath()).enforceTrainingConfig(False).buildSequential()
			End Using

			Dim model As MultiLayerNetwork = kerasModel.MultiLayerNetwork
			model.init()

			Console.WriteLine(model.summary())

			' 1. Layer
			Dim firstLstm As LSTM = DirectCast(model.getLayer(0), LSTM)
			Dim firstConf As org.deeplearning4j.nn.conf.layers.LSTM = CType(firstLstm.conf().getLayer(), org.deeplearning4j.nn.conf.layers.LSTM)
			' "unit_forget_bias": true
			assertTrue(firstConf.getForgetGateBiasInit() = 1.0)

			assertTrue(TypeOf firstConf.getGateActivationFn() Is ActivationHardSigmoid)
			assertTrue(TypeOf firstConf.getActivationFn() Is ActivationTanH)

			Dim nIn As Integer = 12
			Dim nOut As Integer = 96

			' Need to convert from IFCO to CFOI order
			'
			Dim W As INDArray = firstLstm.getParam("W")
			assertTrue(W.shape().SequenceEqual(New Long(){nIn, 4 * nOut}))
			assertEquals(W.getDouble(0, 288), -0.30737767, 1e-7)
			assertEquals(W.getDouble(0, 289), -0.5845409, 1e-7)
			assertEquals(W.getDouble(1, 288), -0.44083247, 1e-7)
			assertEquals(W.getDouble(11, 288), 0.017539706, 1e-7)
			assertEquals(W.getDouble(0, 96), 0.2707935, 1e-7)
			assertEquals(W.getDouble(0, 192), -0.19856165, 1e-7)
			assertEquals(W.getDouble(0, 0), 0.15368782, 1e-7)


			Dim RW As INDArray = firstLstm.getParam("RW")
			assertTrue(RW.shape().SequenceEqual(New Long(){nOut, 4 * nOut}))
			assertEquals(RW.getDouble(0, 288), 0.15112677, 1e-7)


			Dim b As INDArray = firstLstm.getParam("b")
			assertTrue(b.shape().SequenceEqual(New Long(){1, 4 * nOut}))
			assertEquals(b.getDouble(0, 288), -0.36940336, 1e-7) ' Keras I
			assertEquals(b.getDouble(0, 96), 0.6031118, 1e-7) ' Keras F
			assertEquals(b.getDouble(0, 192), -0.13569744, 1e-7) ' Keras O
			assertEquals(b.getDouble(0, 0), -0.2587392, 1e-7) ' Keras C

			' 2. Layer
			Dim secondLstm As LSTM = CType(DirectCast(model.getLayer(1), LastTimeStepLayer).getUnderlying(), LSTM)
			Dim secondConf As org.deeplearning4j.nn.conf.layers.LSTM = CType(secondLstm.conf().getLayer(), org.deeplearning4j.nn.conf.layers.LSTM)
			' "unit_forget_bias": true
			assertTrue(secondConf.getForgetGateBiasInit() = 1.0)

			assertTrue(TypeOf firstConf.getGateActivationFn() Is ActivationHardSigmoid)
			assertTrue(TypeOf firstConf.getActivationFn() Is ActivationTanH)

			nIn = 96
			nOut = 96

			W = secondLstm.getParam("W")
			assertTrue(W.shape().SequenceEqual(New Long(){nIn, 4 * nOut}))
			assertEquals(W.getDouble(0, 288), -0.7559755, 1e-7)

			RW = secondLstm.getParam("RW")
			assertTrue(RW.shape().SequenceEqual(New Long(){nOut, 4 * nOut}))
			assertEquals(RW.getDouble(0, 288), -0.33184892, 1e-7)


			b = secondLstm.getParam("b")
			assertTrue(b.shape().SequenceEqual(New Long(){1, 4 * nOut}))
			assertEquals(b.getDouble(0, 288), -0.2223678, 1e-7)
			assertEquals(b.getDouble(0, 96), 0.73556226, 1e-7)
			assertEquals(b.getDouble(0, 192), -0.63227624, 1e-7)
			assertEquals(b.getDouble(0, 0), 0.06636357, 1e-7)

			Dim dataDir As File = testDir.toFile()

			Dim reader As SequenceRecordReader = New CSVSequenceRecordReader(0, ";")
			Call (New ClassPathResource("deeplearning4j-modelimport/data/", classLoader)).copyDirectory(dataDir)
			reader.initialize(New org.datavec.api.Split.NumberedFileInputSplit(dataDir.getAbsolutePath() & "/sequences/%d.csv", 0, 282))

			Dim dataSetIterator As DataSetIterator = New SequenceRecordReaderDataSetIterator(reader, 1, -1, 12, True)
			Dim preds As IList(Of Double) = New LinkedList(Of Double)()

			Do While dataSetIterator.MoveNext()
				Dim dataSet As DataSet = dataSetIterator.Current
				Dim sequence As INDArray = dataSet.Features.get(NDArrayIndex.point(0)).transpose()
				Dim bsSequence As INDArray = sequence.reshape(ChrW(1), 4, 12) ' one batch
				Dim pred As INDArray = model.output(bsSequence)
				assertTrue(pred.shape().SequenceEqual(New Long(){1, 1}))
				preds.Add(pred.getDouble(0, 0))
			Loop
			Dim dl4jPredictions As INDArray = Nd4j.create(preds)

			Dim kerasPredictions As INDArray = Nd4j.createFromNpyFile(Resources.asFile("modelimport/keras/fullconfigs/lstm/predictions.npy"))

			For i As Integer = 0 To 282
				assertEquals(kerasPredictions.getDouble(i), dl4jPredictions.getDouble(i), 1e-7)
			Next i


			Dim ones As INDArray = Nd4j.ones(1, 4, 12)
			Dim predOnes As INDArray = model.output(ones)
			assertEquals(predOnes.getDouble(0, 0), 0.7216, 1e-4)


		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Disabled("Data and channel layout mismatch. We don't support permuting the weights yet.") public void cnnBatchNormTest() throws IOException, UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub cnnBatchNormTest()

			Dim modelPath As String = "modelimport/keras/fullconfigs/cnn/cnn_batch_norm.h5"

			Dim kerasModel As KerasSequentialModel = (New KerasModel()).modelBuilder().modelHdf5Filename(Resources.asFile(modelPath).getAbsolutePath()).enforceTrainingConfig(False).buildSequential()

			Dim model As MultiLayerNetwork = kerasModel.MultiLayerNetwork
			model.init()

			Console.WriteLine(model.summary())

			Dim input As INDArray = Nd4j.createFromNpyFile(Resources.asFile("modelimport/keras/fullconfigs/cnn/input.npy"))


			Dim output As INDArray = model.output(input)

			Dim kerasOutput As INDArray = Nd4j.createFromNpyFile(Resources.asFile("modelimport/keras/fullconfigs/cnn/predictions.npy"))

			For i As Integer = 0 To 4
				assertEquals(output.getDouble(i), kerasOutput.getDouble(i), 1e-4)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Disabled("Data and channel layout mismatch. We don't support permuting the weights yet.") public void cnnBatchNormLargerTest() throws IOException, UnsupportedKerasConfigurationException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub cnnBatchNormLargerTest()

			Dim modelPath As String = "modelimport/keras/fullconfigs/cnn_batch_norm/cnn_batch_norm_medium.h5"

			Dim kerasModel As KerasSequentialModel = (New KerasModel()).modelBuilder().modelHdf5Filename(Resources.asFile(modelPath).getAbsolutePath()).enforceTrainingConfig(False).buildSequential()

			Dim model As MultiLayerNetwork = kerasModel.MultiLayerNetwork
			model.init()

			Console.WriteLine(model.summary())

			Dim input As INDArray = Nd4j.createFromNpyFile(Resources.asFile("modelimport/keras/fullconfigs/cnn_batch_norm/input.npy"))
			'input = input.permute(0, 3, 1, 2);
			'assertTrue(Arrays.equals(input.shape(), new long[] {5, 1, 48, 48}));

			Dim output As INDArray = model.output(input)

			Dim kerasOutput As INDArray = Nd4j.createFromNpyFile(Resources.asFile("modelimport/keras/fullconfigs/cnn_batch_norm/predictions.npy"))

			For i As Integer = 0 To 4
				' TODO this should be a little closer
				assertEquals(output.getDouble(i), kerasOutput.getDouble(i), 1e-2)
			Next i
		End Sub

	End Class

End Namespace