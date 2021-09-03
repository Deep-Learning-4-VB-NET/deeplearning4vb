﻿'
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

Namespace org.deeplearning4j.ui.api


	Public Class Utils

		Private Sub New()
		End Sub

		''' <summary>
		''' Convert an AWT color to a hex color string, such as #000000 </summary>
		Public Shared Function colorToHex(ByVal color As Color) As String
			Return String.Format("#{0:x2}{1:x2}{2:x2}", color.getRed(), color.getGreen(), color.getBlue())
		End Function

	End Class

End Namespace