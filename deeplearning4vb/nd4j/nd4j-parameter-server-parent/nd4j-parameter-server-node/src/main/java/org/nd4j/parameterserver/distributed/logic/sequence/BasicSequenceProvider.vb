Imports System
Imports SequenceProvider = org.nd4j.parameterserver.distributed.logic.SequenceProvider

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

Namespace org.nd4j.parameterserver.distributed.logic.sequence


	<Obsolete>
	Public Class BasicSequenceProvider
		Implements SequenceProvider

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared ReadOnly INSTANCE_Conflict As New BasicSequenceProvider()
		Private Shared ReadOnly sequence As New AtomicLong(1)

		Private Sub New()

		End Sub

		Public Shared ReadOnly Property Instance As BasicSequenceProvider
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property NextValue As Long? Implements SequenceProvider.getNextValue
			Get
				Return sequence.incrementAndGet()
			End Get
		End Property
	End Class

End Namespace