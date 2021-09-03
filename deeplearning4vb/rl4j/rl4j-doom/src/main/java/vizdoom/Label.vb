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

Namespace vizdoom

	Public Class Label
		Public objectId As Integer
		Public objectName As String
		Public value As SByte
		Public objectPositionX As Double
		Public objectPositionY As Double
		Public objectPositionZ As Double

		Friend Sub New(ByVal id As Integer, ByVal name As String, ByVal value As SByte, ByVal positionX As Double, ByVal positionY As Double, ByVal positionZ As Double)
			Me.objectId = objectId
			Me.objectName = objectName
			Me.value = value
			Me.objectPositionX = positionX
			Me.objectPositionY = positionY
			Me.objectPositionZ = positionZ
		End Sub
	End Class

End Namespace