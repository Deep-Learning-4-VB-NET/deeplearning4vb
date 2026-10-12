Imports System
Imports System.Threading
Imports NonNull = lombok.NonNull
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.nn.util


	Public Class TestDataSetConsumer
		Private iterator As DataSetIterator
		Private delay As Long
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private count_Conflict As New AtomicLong(0)
		Protected Friend Shared ReadOnly logger As Logger = LoggerFactory.getLogger(GetType(TestDataSetConsumer))

		Public Sub New(ByVal delay As Long)
			Me.delay = delay
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public TestDataSetConsumer(@NonNull DataSetIterator iterator, long delay)
		Public Sub New(ByVal iterator As DataSetIterator, ByVal delay As Long)
			Me.iterator = iterator
			Me.delay = delay
		End Sub


		''' <summary>
		''' This method cycles through iterator, whie iterator.hasNext() returns true. After each cycle execution time is simulated either using Thread.sleep() or empty cycle
		''' </summary>
		''' <param name="consumeWithSleep">
		''' @return </param>
		Public Overridable Function consumeWhileHasNext(ByVal consumeWithSleep As Boolean) As Long
			count_Conflict.set(0)
			If iterator Is Nothing Then
				Throw New Exception("Can't use consumeWhileHasNext() if iterator isn't set")
			End If

			Do While iterator.MoveNext()
				Dim ds As DataSet = iterator.Current
				Me.consumeOnce(ds, consumeWithSleep)
			Loop

			Return count_Conflict.get()
		End Function

		''' <summary>
		''' This method consumes single DataSet, and spends delay time simulating execution of this dataset
		''' </summary>
		''' <param name="dataSet"> </param>
		''' <param name="consumeWithSleep">
		''' @return </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public long consumeOnce(@NonNull DataSet dataSet, boolean consumeWithSleep)
		Public Overridable Function consumeOnce(ByVal dataSet As DataSet, ByVal consumeWithSleep As Boolean) As Long
			Dim timeMs As Long = DateTimeHelper.CurrentUnixTimeMillis() + delay
			Do While DateTimeHelper.CurrentUnixTimeMillis() < timeMs
				If consumeWithSleep Then
					Try
						Thread.Sleep(delay)
					Catch e As Exception
						Throw New Exception(e)
					End Try
				End If
			Loop

			count_Conflict.incrementAndGet()

	'        if (count.get() % 100 == 0)
	'            logger.info("Passed {} datasets...", count.get());

			Return count_Conflict.get()
		End Function

		Public Overridable ReadOnly Property Count As Long
			Get
				Return count_Conflict.get()
			End Get
		End Property
	End Class

End Namespace