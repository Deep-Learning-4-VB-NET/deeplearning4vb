Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
Imports MultiNormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerMinMaxScaler
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
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

Namespace org.deeplearning4j.datasets.iterator

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class CombinedPreProcessorTests extends org.deeplearning4j.BaseDL4JTest
	Public Class CombinedPreProcessorTests
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void somePreProcessorsCombined()
		Public Overridable Sub somePreProcessorsCombined()

			Dim featureArr() As INDArray = {Nd4j.linspace(100, 200, 20).reshape(ChrW(10), 2)}
			Dim multiDataSet As New org.nd4j.linalg.dataset.MultiDataSet(featureArr, Nothing, Nothing, Nothing)

			Dim minMaxScaler As New MultiNormalizerMinMaxScaler()
			minMaxScaler.fit(multiDataSet)
			Dim multiDataSetPreProcessor As CombinedMultiDataSetPreProcessor = (New CombinedMultiDataSetPreProcessor.Builder()).addPreProcessor(minMaxScaler).addPreProcessor(1, New addFivePreProcessor(Me)).build()

			multiDataSetPreProcessor.preProcess(multiDataSet)
			assertEquals(Nd4j.zeros(10, 2).addColumnVector(Nd4j.linspace(0, 1, 10).reshape(ChrW(10), 1)).addi(5), multiDataSet.getFeatures(0))

		End Sub

	'    
	'        Adds five to the features - assumes multidataset here is one feature and one label
	'     
		Public NotInheritable Class addFivePreProcessor
			Implements MultiDataSetPreProcessor

			Private ReadOnly outerInstance As CombinedPreProcessorTests

			Public Sub New(ByVal outerInstance As CombinedPreProcessorTests)
				Me.outerInstance = outerInstance
			End Sub


			Public Sub preProcess(ByVal multiDataSet As MultiDataSet)
				multiDataSet.getFeatures(0).addi(5)
			End Sub
		End Class
	End Class

End Namespace