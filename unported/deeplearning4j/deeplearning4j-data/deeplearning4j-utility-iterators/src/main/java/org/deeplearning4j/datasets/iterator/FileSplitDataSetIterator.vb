Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FileCallback = org.deeplearning4j.datasets.iterator.callbacks.FileCallback
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

Namespace org.deeplearning4j.datasets.iterator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class FileSplitDataSetIterator implements org.nd4j.linalg.dataset.api.iterator.DataSetIterator
	<Serializable>
	Public Class FileSplitDataSetIterator
		Implements DataSetIterator

'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private preProcessor_Conflict As DataSetPreProcessor

		Private files As IList(Of File)
		Private numFiles As Integer
		Private counter As New AtomicInteger(0)
		Private callback As FileCallback

		''' <param name="files">    List of files to iterate over </param>
		''' <param name="callback"> Callback for loading the files </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public FileSplitDataSetIterator(@NonNull List<java.io.File> files, @NonNull FileCallback callback)
		Public Sub New(ByVal files As IList(Of File), ByVal callback As FileCallback)
			Me.files = files
			Me.numFiles = files.size()
			Me.callback = callback
		End Sub


		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Throw New System.NotSupportedException()
		End Function

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return 0
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return 0
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return True
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return True
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			counter.set(0)
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return 0
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
				Return Nothing
			End Get
		End Property

		Public Overrides Function hasNext() As Boolean
			Return counter.get() < numFiles
		End Function

		Public Overrides Function [next]() As DataSet
			'        long time1 = System.nanoTime();
			Dim ds As DataSet = callback.call(files(counter.getAndIncrement()))

			If preProcessor_Conflict IsNot Nothing AndAlso ds IsNot Nothing Then
				preProcessor_Conflict.preProcess(ds)
			End If

			'        long time2 = System.nanoTime();

			'        if (counter.get() % 5 == 0)
			'            log.info("Device: [{}]; Time: [{}] ns;", Nd4j.getAffinityManager().getDeviceForCurrentThread(), time2 - time1);

			Return ds
		End Function

		Public Overrides Sub remove()
			' no-op
		End Sub
	End Class

End Namespace