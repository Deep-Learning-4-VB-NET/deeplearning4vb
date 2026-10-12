Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.deeplearning4j.nn.graph.util


	Public Class ComputationGraphUtil

		Private Sub New()
		End Sub

		''' <summary>
		''' Convert a DataSet to the equivalent MultiDataSet </summary>
		Public Shared Function toMultiDataSet(ByVal dataSet As DataSet) As MultiDataSet
			Dim f As INDArray = dataSet.Features
			Dim l As INDArray = dataSet.Labels
			Dim fMask As INDArray = dataSet.FeaturesMaskArray
			Dim lMask As INDArray = dataSet.LabelsMaskArray
			Dim meta As IList(Of Serializable) = dataSet.getExampleMetaData()

			Dim fNew() As INDArray = If(f Is Nothing, Nothing, New INDArray()){f}
			Dim lNew() As INDArray = If(l Is Nothing, Nothing, New INDArray()){l}
			Dim fMaskNew() As INDArray = (If(fMask IsNot Nothing, New INDArray() {fMask}, Nothing))
			Dim lMaskNew() As INDArray = (If(lMask IsNot Nothing, New INDArray() {lMask}, Nothing))

			Dim mds As New org.nd4j.linalg.dataset.MultiDataSet(fNew, lNew, fMaskNew, lMaskNew)
			mds.ExampleMetaData = meta
			Return mds
		End Function

		''' <summary>
		''' Convert a DataSetIterator to a MultiDataSetIterator, via an adaptor class </summary>
		Public Shared Function toMultiDataSetIterator(ByVal iterator As DataSetIterator) As MultiDataSetIterator
			Return New MultiDataSetIteratorAdapter(iterator)
		End Function

	End Class

End Namespace