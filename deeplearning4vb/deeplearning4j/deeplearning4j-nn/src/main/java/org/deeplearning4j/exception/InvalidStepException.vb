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

Namespace org.deeplearning4j.exception

	Public Class InvalidStepException
		Inherits Exception

		''' <summary>
		''' Constructs a new exception with the specified detail message.  The
		''' cause is not initialized, and may subsequently be initialized by
		''' a call to <seealso cref="initCause"/>.
		''' </summary>
		''' <param name="message"> the detail message. The detail message is saved for
		'''                later retrieval by the <seealso cref="getMessage()"/> method. </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified detail message and
		''' cause.  <para>Note that the detail message associated with
		''' {@code cause} is <i>not</i> automatically incorporated in
		''' this exception's detail message.
		''' 
		''' </para>
		''' </summary>
		''' <param name="message"> the detail message (which is saved for later retrieval
		'''                by the <seealso cref="getMessage()"/> method). </param>
		''' <param name="cause">   the cause (which is saved for later retrieval by the
		'''                <seealso cref="getCause()"/> method).  (A <tt>null</tt> value is
		'''                permitted, and indicates that the cause is nonexistent or
		'''                unknown.)
		''' @since 1.4 </param>
		Public Sub New(ByVal message As String, ByVal cause As Exception)
			MyBase.New(message, cause)
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified cause and a detail
		''' message of <tt>(cause==null ? null : cause.toString())</tt> (which
		''' typically contains the class and detail message of <tt>cause</tt>).
		''' This constructor is useful for exceptions that are little more than
		''' wrappers for other throwables (for example, {@link
		''' java.security.PrivilegedActionException}).
		''' </summary>
		''' <param name="cause"> the cause (which is saved for later retrieval by the
		'''              <seealso cref="getCause()"/> method).  (A <tt>null</tt> value is
		'''              permitted, and indicates that the cause is nonexistent or
		'''              unknown.)
		''' @since 1.4 </param>
		Public Sub New(ByVal cause As Exception)
			MyBase.New(cause)
		End Sub

		''' <summary>
		''' Constructs a new exception with the specified detail message,
		''' cause, suppression enabled or disabled, and writable stack
		''' trace enabled or disabled.
		''' </summary>
		''' <param name="message">            the detail message. </param>
		''' <param name="cause">              the cause.  (A {@code null} value is permitted,
		'''                           and indicates that the cause is nonexistent or unknown.) </param>
		''' <param name="enableSuppression">  whether or not suppression is enabled
		'''                           or disabled </param>
		''' <param name="writableStackTrace"> whether or not the stack trace should
		'''                           be writable
		''' @since 1.7 </param>
		Protected Friend Sub New(ByVal message As String, ByVal cause As Exception, ByVal enableSuppression As Boolean, ByVal writableStackTrace As Boolean)
			MyBase.New(message, cause, enableSuppression, writableStackTrace)
		End Sub
	End Class

End Namespace