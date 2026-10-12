Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator

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

Namespace org.deeplearning4j.spark.parameterserver.iterators


	<Serializable>
	Public Class VirtualDataSetIterator
		Implements DataSetIterator

		''' <summary>
		''' Basic idea here is simple: this DataSetIterator will take in multiple lazy Iterator<DataSet>,
		''' and will push them is round-robin manner to ParallelWrapper workers
		''' </summary>

		Protected Friend ReadOnly iterators As IList(Of IEnumerator(Of DataSet))
		Protected Friend ReadOnly position As AtomicInteger

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public VirtualDataSetIterator(@NonNull List<java.util.Iterator<org.nd4j.linalg.dataset.DataSet>> iterators)
		Public Sub New(ByVal iterators As IList(Of IEnumerator(Of DataSet)))
			Me.iterators = iterators
			Me.position = New AtomicInteger(0)
		End Sub

	'    
	'    
	'    // TODO: to be implemented
	'    
	'    @Override
	'    public void attachThread(int producer) {
	'        throw new UnsupportedOperationException();
	'    }
	'    
	'    @Override
	'    public boolean hasNextFor() {
	'        return false;
	'    }
	'    
	'    @Override
	'    public boolean hasNextFor(int consumer) {
	'        return false;
	'    }
	'    
	'    @Override
	'    public DataSet nextFor(int consumer) {
	'        return null;
	'    }
	'    
	'    @Override
	'    public DataSet nextFor() {
	'        return null;
	'    }
	'    
	'    
		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			' we're NOT supporting reset() here
			Return False
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
    
			End Set
			Get
				' we probably don't need this thing here
				Return Nothing
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			' just checking if that's not the last iterator, or if that's the last one - check if it has something
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return position.get() < iterators.Count - 1 OrElse (position.get() < iterators.Count AndAlso iterators(position.get()).hasNext())
		End Function

		Public Overrides Function [next]() As DataSet
			' TODO: this solution isn't ideal, it assumes non-empty iterators all the time. Would be nice to do something here
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterators(position.get()).hasNext() Then
				position.getAndIncrement()
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iterators(position.get()).next()
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub

		Public Overridable Sub reset() Implements DataSetIterator.reset
			Throw New System.NotSupportedException()
		End Sub


		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Return Nothing
			End Get
		End Property
	End Class

End Namespace