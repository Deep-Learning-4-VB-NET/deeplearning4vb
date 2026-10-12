Imports System
Imports System.IO

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

Namespace org.deeplearning4j.models.word2vec


	<Serializable>
	Public Class StreamWork
'JAVA TO VB CONVERTER NOTE: The field is was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private is_Conflict As InputStreamCreator
'JAVA TO VB CONVERTER NOTE: The field count was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private count_Conflict As New AtomicInteger(0)


		Public Sub New(ByVal [is] As InputStreamCreator, ByVal count As AtomicInteger)
			MyBase.New()
			Me.is_Conflict = [is]
			Me.count_Conflict = count
		End Sub

		Public Overridable ReadOnly Property Is As Stream
			Get
				Return is_Conflict.create()
			End Get
		End Property

		Public Overridable Property Count As AtomicInteger
			Get
				Return count_Conflict
			End Get
			Set(ByVal count As AtomicInteger)
				Me.count_Conflict = count
			End Set
		End Property


		Public Overridable Sub countDown()
			count_Conflict.decrementAndGet()

		End Sub



	End Class

End Namespace