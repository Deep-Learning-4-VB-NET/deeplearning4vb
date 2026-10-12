Imports System
Imports System.Collections.Generic
Imports Model = org.deeplearning4j.nn.api.Model
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener

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

Namespace org.deeplearning4j.optimize.listeners


	<Obsolete, Serializable>
	Public Class ComposableIterationListener
		Inherits BaseTrainingListener

		Private listeners As ICollection(Of TrainingListener) = New List(Of TrainingListener)()

		Public Sub New(ParamArray ByVal TrainingListener() As TrainingListener)
			listeners.addAll(Arrays.asList(TrainingListener))
		End Sub

		Public Sub New(ByVal listeners As ICollection(Of TrainingListener))
			Me.listeners = listeners
		End Sub

		Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
			For Each listener As TrainingListener In listeners
				listener.iterationDone(model, iteration, epoch)
			Next listener
		End Sub
	End Class

End Namespace