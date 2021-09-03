Imports System
Imports System.Collections.Generic
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports RocCurve = org.nd4j.evaluation.curves.RocCurve
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
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
Namespace org.deeplearning4j.eval


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Roc Test") @NativeTag @Tag(TagNames.EVAL_METRICS) @Tag(TagNames.JACKSON_SERDE) @Tag(TagNames.TRAINING) @Tag(TagNames.DL4J_OLD_API) class ROCTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ROCTest
		Inherits BaseDL4JTest

		Private Shared expTPR As IDictionary(Of Double, Double)

		Private Shared expFPR As IDictionary(Of Double, Double)

		Shared Sub New()
			expTPR = New Dictionary(Of Double, Double)()
			Dim totalPositives As Double = 5.0
			' All 10 predicted as class 1, of which 5 of 5 are correct
			expTPR(0 / 10.0) = 5.0 / totalPositives
			expTPR(1 / 10.0) = 5.0 / totalPositives
			expTPR(2 / 10.0) = 5.0 / totalPositives
			expTPR(3 / 10.0) = 5.0 / totalPositives
			expTPR(4 / 10.0) = 5.0 / totalPositives
			expTPR(5 / 10.0) = 5.0 / totalPositives
			' Threshold: 0.4 -> last 4 predicted; last 5 actual
			expTPR(6 / 10.0) = 4.0 / totalPositives
			expTPR(7 / 10.0) = 3.0 / totalPositives
			expTPR(8 / 10.0) = 2.0 / totalPositives
			expTPR(9 / 10.0) = 1.0 / totalPositives
			expTPR(10 / 10.0) = 0.0 / totalPositives
			expFPR = New Dictionary(Of Double, Double)()
			Dim totalNegatives As Double = 5.0
			' All 10 predicted as class 1, but all 5 true negatives are predicted positive
			expFPR(0 / 10.0) = 5.0 / totalNegatives
			' 1 true negative is predicted as negative; 4 false positives
			expFPR(1 / 10.0) = 4.0 / totalNegatives
			' 2 true negatives are predicted as negative; 3 false positives
			expFPR(2 / 10.0) = 3.0 / totalNegatives
			expFPR(3 / 10.0) = 2.0 / totalNegatives
			expFPR(4 / 10.0) = 1.0 / totalNegatives
			expFPR(5 / 10.0) = 0.0 / totalNegatives
			expFPR(6 / 10.0) = 0.0 / totalNegatives
			expFPR(7 / 10.0) = 0.0 / totalNegatives
			expFPR(8 / 10.0) = 0.0 / totalNegatives
			expFPR(9 / 10.0) = 0.0 / totalNegatives
			expFPR(10 / 10.0) = 0.0 / totalNegatives
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Roc Eval Sanity Check") void RocEvalSanityCheck()
		Friend Overridable Sub RocEvalSanityCheck()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			Nd4j.Random.setSeed(12345)
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).seed(12345).list().layer(0, (New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build()).layer(1, (New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim ns As New NormalizerStandardize()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = iter.next()
			ns.fit(ds)
			ns.transform(ds)
			iter.PreProcessor = ns
			For i As Integer = 0 To 9
				net.fit(ds)
			Next i
			For Each steps As Integer In New Integer() { 32, 0 }
				' Steps = 0: exact
				Console.WriteLine("steps: " & steps)
				iter.reset()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				ds = iter.next()
				Dim f As INDArray = ds.Features
				Dim l As INDArray = ds.Labels
				Dim [out] As INDArray = net.output(f)
				' System.out.println(f);
				' System.out.println(out);
				Dim manual As New ROCMultiClass(steps)
				manual.eval(l, [out])
				iter.reset()
				Dim roc As ROCMultiClass = net.evaluateROCMultiClass(iter, steps)
				For i As Integer = 0 To 2
					Dim rocExp As Double = manual.calculateAUC(i)
					Dim rocAct As Double = roc.calculateAUC(i)
					assertEquals(rocExp, rocAct, 1e-6)
					Dim rc As RocCurve = roc.getRocCurve(i)
					Dim rm As RocCurve = manual.getRocCurve(i)
					assertEquals(rc, rm)
				Next i
			Next steps
		End Sub
	End Class

End Namespace