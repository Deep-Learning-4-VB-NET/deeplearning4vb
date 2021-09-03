Imports System
Imports IntArrayList = it.unimi.dsi.fastutil.ints.IntArrayList
Imports Data = lombok.Data
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor
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

Namespace org.deeplearning4j.integration.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class CountingMultiDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator
	<Serializable>
	Public Class CountingMultiDataSetIterator
		Implements MultiDataSetIterator

		Private underlying As MultiDataSetIterator
		Private currIter As Integer = 0
		Private iterAtReset As New IntArrayList()
		Private tbptt As Boolean
		Private tbpttLength As Integer

		''' 
		''' <param name="underlying">  Underlying iterator </param>
		''' <param name="tbptt">       Whether TBPTT is used </param>
		''' <param name="tbpttLength"> Network TBPTT length </param>
		Public Sub New(ByVal underlying As MultiDataSetIterator, ByVal tbptt As Boolean, ByVal tbpttLength As Integer)
			Me.underlying = underlying
			Me.tbptt = tbptt
			Me.tbpttLength = tbpttLength
		End Sub

		Public Overridable Function [next](ByVal i As Integer) As MultiDataSet Implements MultiDataSetIterator.next
			currIter += 1
			Return underlying.next(i)
		End Function

		Public Overridable Property PreProcessor Implements MultiDataSetIterator.setPreProcessor As MultiDataSetPreProcessor
			Set(ByVal multiDataSetPreProcessor As MultiDataSetPreProcessor)
				underlying.PreProcessor = multiDataSetPreProcessor
			End Set
			Get
				Return underlying.PreProcessor
			End Get
		End Property


		Public Overridable Function resetSupported() As Boolean Implements MultiDataSetIterator.resetSupported
			Return underlying.resetSupported()
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements MultiDataSetIterator.asyncSupported
			Return underlying.asyncSupported()
		End Function

		Public Overridable Sub reset() Implements MultiDataSetIterator.reset
			underlying.reset()
			iterAtReset.add(currIter)
			currIter = 0
		End Sub

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return underlying.hasNext()
		End Function

		Public Overrides Function [next]() As MultiDataSet
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim mds As MultiDataSet = underlying.next()
			If tbptt Then
				Dim f As INDArray = mds.getFeatures(0)
				If f.rank() = 3 Then
					Dim numSegments As Integer = CInt(Math.Truncate(Math.Ceiling(f.size(2) / CDbl(tbpttLength))))
					currIter += numSegments
				End If
			Else
				currIter += 1
			End If
			Return mds
		End Function
	End Class

End Namespace