Imports System
Imports System.Collections.Generic
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
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

Namespace org.deeplearning4j.datasets.iterator


	<Serializable>
	Public Class MultiDataSetWrapperIterator
		Implements DataSetIterator

		Protected Friend iterator As MultiDataSetIterator
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preProcessor_Conflict As DataSetPreProcessor

		''' <param name="iterator"> Undelying iterator to wrap </param>
		Public Sub New(ByVal iterator As MultiDataSetIterator)
			Me.iterator = iterator
		End Sub

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return iterator.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return iterator.asyncSupported()
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			iterator.reset()
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
			Get
				Return preProcessor_Conflict
			End Get
		End Property


		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iterator.hasNext()
		End Function

		Public Overrides Function [next]() As DataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = iterator.next()
			If mds.Features.Length > 1 OrElse mds.Labels.Length > 1 Then
				Throw New System.NotSupportedException("This iterator is able to convert MultiDataSet with number of inputs/outputs of 1")
			End If

			Dim features As INDArray = mds.Features(0)
			Dim labels As INDArray = If(mds.Labels IsNot Nothing, mds.Labels(0), features)
			Dim fMask As INDArray = If(mds.FeaturesMaskArrays IsNot Nothing, mds.FeaturesMaskArrays(0), Nothing)
			Dim lMask As INDArray = If(mds.LabelsMaskArrays IsNot Nothing, mds.LabelsMaskArrays(0), Nothing)

			Dim ds As New DataSet(features, labels, fMask, lMask)

			If preProcessor_Conflict IsNot Nothing Then
				preProcessor_Conflict.preProcess(ds)
			End If

			Return ds
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub
	End Class

End Namespace