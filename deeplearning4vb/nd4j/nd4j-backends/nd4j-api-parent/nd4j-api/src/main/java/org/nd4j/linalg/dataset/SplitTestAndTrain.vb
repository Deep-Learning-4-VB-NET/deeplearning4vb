Imports System

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

Namespace org.nd4j.linalg.dataset


	''' <summary>
	''' SplitV test and train
	''' 
	''' @author Adam Gibson
	''' </summary>
	<Serializable>
	Public Class SplitTestAndTrain

'JAVA TO VB CONVERTER NOTE: The field train was renamed since Visual Basic does not allow fields to have the same name as other class members:
'JAVA TO VB CONVERTER NOTE: The field test was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private train_Conflict, test_Conflict As DataSet

		Public Sub New(ByVal train As DataSet, ByVal test As DataSet)
			Me.train_Conflict = train
			Me.test_Conflict = test
		End Sub

		Public Overridable Property Test As DataSet
			Get
				Return test_Conflict
			End Get
			Set(ByVal test As DataSet)
				Me.test_Conflict = test
			End Set
		End Property


		Public Overridable Property Train As DataSet
			Get
				Return train_Conflict
			End Get
			Set(ByVal train As DataSet)
				Me.train_Conflict = train
			End Set
		End Property

	End Class

End Namespace