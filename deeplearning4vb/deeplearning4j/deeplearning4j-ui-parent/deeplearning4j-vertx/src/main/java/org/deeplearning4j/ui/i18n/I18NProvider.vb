Imports I18N = org.deeplearning4j.ui.api.I18N

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

Namespace org.deeplearning4j.ui.i18n


	Public Class I18NProvider

		''' <summary>
		''' Current I18N instance
		''' </summary>
		Private Shared i18n As I18N = DefaultI18N.Instance

		''' <summary>
		''' Get the current/global I18N instance (used in single-session mode)
		''' </summary>
		''' <returns> global instance </returns>
		Public Shared ReadOnly Property Instance As I18N
			Get
				Return i18n
			End Get
		End Property


		''' <summary>
		''' Get instance for session (used in multi-session mode)
		''' </summary>
		''' <param name="sessionId"> session </param>
		''' <returns> instance for session </returns>
		Public Shared Function getInstance(ByVal sessionId As String) As I18N
			Return DefaultI18N.getInstance(sessionId)
		End Function

		''' <summary>
		''' Remove I18N instance for session
		''' </summary>
		''' <param name="sessionId"> session ID </param>
		''' <returns> the previous value associated with {@code sessionId} or null if there was no mapping for {@code sessionId} </returns>
		Public Shared Function removeInstance(ByVal sessionId As String) As I18N
			SyncLock GetType(I18NProvider)
				Return DefaultI18N.removeInstance(sessionId)
			End SyncLock
		End Function

	End Class

End Namespace